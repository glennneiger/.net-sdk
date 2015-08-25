﻿namespace DreamFactory.Demo.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DreamFactory.Model;
    using DreamFactory.Rest;

    public class DiscoveryDemo : IRunnable
    {
        public async Task RunAsync(IRestContext context)
        {
            // List available services
            IEnumerable<Service> services = context.GetServicesAsync().Result;
            Console.WriteLine("Available services:");
            foreach (Service service in services)
            {
                Console.WriteLine("{0}:\t{1}", service.ApiName, service.Name);
            }

            // List resources
            IEnumerable<Resource> resources = await context.GetResourcesAsync("user");
            Console.WriteLine();
            Console.WriteLine("/user resources:");
            foreach (Resource resource in resources)
            {
                Console.WriteLine("\t/{0}", resource.Name);
            }

            resources = await context.GetResourcesAsync("files");
            Console.WriteLine();
            Console.WriteLine("/files resources:");
            foreach (Resource resource in resources)
            {
                Console.WriteLine("\t/{0}", resource.Name);
            }
        }
    }
}
