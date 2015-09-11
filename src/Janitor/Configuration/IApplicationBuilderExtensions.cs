using System;
using IdentityServer3.Core.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Janitor;
using Owin;
using Microsoft.Framework.DependencyInjection;
using Owin.Security.AesDataProtectorProvider;

namespace Microsoft.AspNet.Builder
{
    using IdentityManager.Configuration;
    using DataProtectionProviderDelegate = Func<string[], Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>>;
    using DataProtectionTuple = Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>;

    public static class IApplicationBuilderExtensions
    {
        public static void UseIdentityServer(this IApplicationBuilder app, IdentityServerOptions options)
        {
            
            app.UseOwin(pipeline =>
            {
                pipeline(next =>
                {
                    var builder = new Microsoft.Owin.Builder.AppBuilder();
                    var provider = app.ApplicationServices.GetService<DataProtection.IDataProtectionProvider>();

                    builder.Properties["security.DataProtectionProvider"] = new DataProtectionProviderDelegate(purposes =>
                    {
                        var dataProtection = provider.CreateProtector(String.Join(",", purposes));
                        return new DataProtectionTuple(dataProtection.Protect, dataProtection.Unprotect);
                    });

                    builder.Map("/admin", adminApp =>
                    {
                        var factory = new IdentityManagerServiceFactory();
                        factory.ConfigureSimpleIdentityManagerService("AspId");
                        //factory.ConfigureCustomIdentityManagerServiceWithIntKeys("AspId_CustomPK");

                        adminApp.UseIdentityManager(new IdentityManagerOptions()
                        {
                            Factory = factory
                        });
                    });

                    builder.UseIdentityServer(options);
                    builder.UseAesDataProtectorProvider();
                    var appFunc = builder.Build(typeof(Func<IDictionary<string, object>, Task>)) as Func<IDictionary<string, object>, Task>;
                    return appFunc;
                });
            });
        }
    }
}