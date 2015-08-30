﻿namespace DreamFactory.Demo.Demo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using DreamFactory.Api;
    using DreamFactory.Model.Database;
    using DreamFactory.Model.System.App;
    using DreamFactory.Model.System.Config;
    using DreamFactory.Model.System.Role;
    using DreamFactory.Model.System.Service;
    using DreamFactory.Model.System.User;
    using DreamFactory.Rest;

    public class SystemDemo : IRunnable
    {
        public async Task RunAsync(IRestContext context)
        {
            // IUserApi provides all functions for user management
            ISystemApi systemApi = context.Factory.CreateSystemApi();

            // List apps
            SqlQuery query = new SqlQuery { Filter = "is_active=true", Fields = "*", Related = "service_by_storage_service_id,role_by_user_to_app_to_role", };
            IEnumerable<AppResponse> apps = (await systemApi.GetAppsAsync(query)).ToList();
            Console.WriteLine("Apps: {0}", apps.Select(x => x.Name).ToStringList());

            // Check admin app
            AppResponse adminApp = apps.Single(x => x.Name == "admin");
            RelatedRole role = adminApp.Roles.First();
            Console.WriteLine("admin app has first role: {0}", role.Name);
            RelatedService service = adminApp.StorageService;
            Console.WriteLine("admin app has {0} {1}", service != null ? "service" : "no service", service != null ? service.Name : "");

            // List users with roles
            IEnumerable<UserResponse> users = await systemApi.GetUsersAsync(new SqlQuery());
            Console.WriteLine("Users: {0}", users.Select(x => x.DisplayName).ToStringList());

            // Download app package & SDK
            Console.WriteLine("Downloading app package and SDK...");
            byte[] package = await systemApi.DownloadApplicationPackageAsync(1);
            byte[] sdk = await systemApi.DownloadApplicationSdkAsync(1);
            File.WriteAllBytes("admin-package.zip", package);
            File.WriteAllBytes("admin-sdk.zip", sdk);

            // Get environment info - does not work for WAMP, uncomment when using linux hosted DSP.
            // EnvironmentResponse environment = await systemApi.GetEnvironmentAsync();
            // Console.WriteLine("DreamFactory Server is running on {0}", environment.server.server_os);
            
            // Get constant //TODO: see about constants
            //var contentTypes = await systemApi.GetConstantAsync("content_types");
            //Console.WriteLine("Content Types: {0}", contentTypes.Keys.ToStringList());
        }
    }
}
