using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace UIFT.Security
{
    public static class UIFTAuthMiddlewareSt
    {
        public static void UseUiftAuth(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<UIFTAuthMiddleware>();
        }
    }

    public class UIFTAuthMiddleware
    {
        private readonly ILogger<UIFTAuthMiddleware> Log;
        private readonly RequestDelegate _next;
        private readonly LinkGenerator _linkGenerator;
        private readonly AppConfiguration Configuration;

        public UIFTAuthMiddleware(RequestDelegate next, LinkGenerator generator, AppConfiguration configuration, ILogger<UIFTAuthMiddleware> log)
        {
            Log = log;
            Configuration = configuration;
            _linkGenerator = generator;
            _next = next;
        }

        public async Task Invoke(HttpContext context, UIFT.Repository.RepositoryFactory factory, BO.RunningUser runningUser, AppConfiguration config)
        {
            var executingEnpoint = context.GetEndpoint();
            if (executingEnpoint != null)
            {
                if (executingEnpoint.Metadata == null || !executingEnpoint.Metadata.Any(t => t.GetType() == typeof(AllowAnonymousAttribute)))
                {
                    // a11id
                    int a11id = get_a11id(context);

                    // username
                    var result = GetLogin(context, runningUser);
                    if (result.FailedCode == 0)
                    {
                        var repository = factory.Get(a11id);
                        repository.BL.InhaleUserByLogin(runningUser.j03Login);

                        bool isPreview = executingEnpoint.Metadata.Any(t => t.GetType() == typeof(IsPreviewAttribute));

                        CreateUser(repository, isPreview);
                    }
                    else
                    {
                        //Filipe, tady je ta CHYBA!!!
                        Log.LogWarning("if (result.FailedCode == 0): result.FailedCode {0}; runningUser.j03Login: {1};", result.FailedCode, runningUser.j03Login);
                    }

                    // problem v kontrole prihlaseni
                    if (!result.Success)
                    {
                        // pro ajaxove pozadavky vracej jen 403
                        if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            string message = "Ajax Request: Access Denied";
                            context.Response.StatusCode = 403;
                            //await context.Response.Body.WriteAsync(message, 0, message.Length);
                            return;
                        }
                        else
                        {
                            string url = _linkGenerator.GetPathByAction("Index", "Error", new { code = result.FailedCode }, Configuration.BaseUrl);
                            context.Response.Redirect(url, false);
                            return;
                        }
                    }
                    else // uspesne prihlaseni
                    {
                        context.User = result.User;
                        context.Items.Add("PersistantData", result.PersistantStorage);
                    }

                    void CreateUser(UIFT.Repository.Repository repository, bool preview)
                    {
                        // uzivatel v BL neexistuje
                        if (repository.BL.CurrentUser == null)
                        {
                            result.FailedCode = 4;
                        }
                        // uzivatel uz neni platny
                        else if (repository.BL.CurrentUser.isclosed)
                        {
                            result.FailedCode = 5;
                        }
                        else // platny uzivatel
                        {
                            // informace o udalosti
                            BO.a11EventForm ev = repository.GetBLEvent(a11id);

                            // neexistuje event
                            if (ev == null)
                            {
                                result.FailedCode = 1;
                            }
                            else
                            {
                                // a01event a prava na a01
                                var ev01 = repository.BL.a01EventBL.Load(ev.a01ID);
                                var ev01permission = repository.BL.a01EventBL.InhalePermission(ev01);

                                // vytvoreni instance uzivatele
                                result.User = new UIFTUser(new ClaimsIdentity())
                                {
                                    ID = repository.BL.CurrentUser.pid,
                                    Permissions = repository.BL.CurrentUser.j04RoleValue,
                                    Name = repository.BL.CurrentUser.j03Login,
                                    Jmeno = repository.BL.CurrentUser.FullName
                                };

                                // akce je uzavrena
                                if (ev.f06IsA01ClosedStrict)
                                {
                                    if (ev01.isclosed)
                                    {
                                        preview = true;
                                    }
                                }

                                // formular je mozne vyplnovat pouze v aktualnim terminu
                                if (ev.f06IsA01PeriodStrict &&
                                    ((DateTime.Now > ev01.a01DateUntil || DateTime.Now < ev01.a01DateFrom) || ev01.isclosed))
                                {
                                    preview = true;
                                    /*result.FailedCode = 15;

                                    log.WarnFormat("AuthorizeRequest: Failed {0}; preview: {1}; a01DateUntil: {2}; a01DateFrom: {3}; IsClosed: {4};", result.FailedCode, preview, ev01.a01DateUntil, ev01.a01DateFrom, ev01.IsClosed);
                                    return result;*/
                                }

                                if (ev == null)
                                {
                                    // a11id neexistuje
                                    result.FailedCode = 1;

                                    Log.LogWarning("AuthorizeRequest: Failed {0}; preview: {1};", result.FailedCode, preview);
                                }
                                else
                                {
                                    // udalost je zamknuta ??
                                    if (ev.a11IsLocked)
                                    {
                                        preview = true;
                                    }

                                    // udalost je uzavrena ??
                                    if (ev.isclosed && !preview)
                                    {
                                        result.FailedCode = 3;

                                        Log.LogWarning("AuthorizeRequest: Failed {0}; preview: {1}; user: {2}; a11id: {3}", result.FailedCode, preview, repository.BL.CurrentUser.j03Login, ev.a11ID);
                                    }
                                    // kontrola prav na vyplnovani
                                    else if ((!ev01permission.HasPerm(BO.a01EventPermissionENUM.ShareTeam_Member) && !ev01permission.HasPerm(BO.a01EventPermissionENUM.ShareTeam_Leader) && !ev.a11IsPoll && !preview) ||  // nema vubec pravo na formular
                                        (!ev01permission.HasPerm(BO.a01EventPermissionENUM.ReadOnlyAccess) && ev.a11IsPoll && preview)) // pokud se jedna o Anketu, muze ji editovat kazdy, ale zobrazovat Preview jen podle prav                                        
                                    {
                                        result.FailedCode = 18;

                                        Log.LogWarning("AuthorizeRequest: Failed {0}; ev01permission: {1}; a11IsPoll: {2}; preview: {3}; user: {4}; a11id: {5}", result.FailedCode, ev01permission, ev.a11IsPoll, preview, repository.BL.CurrentUser.j03Login, ev.a11ID);
                                    }
                                    // anonymni uzivatel (tj. uzivatel prihlaseny pres Login/PIN) nema pravo zobrazovat Preview
                                    else if (config.UIFT_AnonymousUser == repository.BL.CurrentUser.j03Login && preview)
                                    {
                                        result.FailedCode = 16;

                                        Log.LogWarning("AuthorizeRequest: Failed {0}; identity.Name: {1}; preview: {2};", result.FailedCode, repository.BL.CurrentUser.j03Login, preview);
                                    }
                                    else
                                    {
                                        // nema pravo na zapis - prepni na Preview
                                        if (!ev.a11IsPoll && ev01permission.HasPerm(BO.a01EventPermissionENUM.ReadOnlyAccess))
                                        {
                                            Log.LogInformation("AuthorizeRequest: Success, but switched to PREVIEW; a11IsPoll: {0}; ev01permission: {1}; User: {2};", ev.a11IsPoll, ev01permission, repository.BL.CurrentUser.j03Login);
                                            preview = true;
                                        }

                                        // uzivatel je prihlasen
                                        result.Success = true;

                                        // persistant data
                                        result.PersistantStorage = new PersistantDataStorage
                                        {
                                            IsEncrypted = !repository.BL.a11EventFormBL.TestUserAccessToEncryptedFormValues(a11id, ev.f06ID),
                                            a11IsPoll = ev.a11IsPoll,
                                            a11id = a11id,
                                            a01id = ev.a01ID,
                                            f06id = ev.f06ID,
                                            Layout = FormularLayoutTypes.Default,
                                            Template = FormularTemplateTypes.Default,
                                            IsPreview = preview,
                                            FormLockFlag = (BO.f06UserLockFlagEnum)ev.f06UserLockFlag
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
            }

            await _next.Invoke(context);
        }

        private AuthorizeRequestResult GetLogin(HttpContext context, BO.RunningUser runningUser)
        {
            var result = new AuthorizeRequestResult();
            // login uzivatele zjisteny z Membership
            if (context.User?.Identity == null) // neautentikovany uzivatel
            {
                result.FailedCode = 14;
            }
            else if (context.User.Identity.IsAuthenticated)
            {
                runningUser.j03Login = context.User.Identity.Name;
                // orezat nazev domeny z loginu
                if (runningUser.j03Login.LastIndexOf("\\") > 0)
                {
                    runningUser.j03Login = runningUser.j03Login.Substring(runningUser.j03Login.LastIndexOf("\\") + 1);
                }
            }
            else // neautentikovany uzivatel
            {
                result.FailedCode = 14;
            }

            return result;
        }

        private int get_a11id(HttpContext context)
        {
            if (context.GetRouteData().Values.ContainsKey("a11id"))
            {
                return Convert.ToInt32(context.GetRouteData().Values["a11id"]);
            }
            else if (context.Request.Query.ContainsKey("a11id"))
            {
                return Convert.ToInt32(context.Request.Query["a11id"]);
            }
            else if (context.Items.ContainsKey("PersistantData"))
            {
                return ((PersistantDataStorage)context.Items["PersistantData"]).a11id;
            }
            else
            {
                return 0;
            }
        }
    }
}
