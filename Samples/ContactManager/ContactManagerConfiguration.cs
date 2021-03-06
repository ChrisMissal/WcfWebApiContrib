﻿// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

using Http.Formatters;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace ContactManager
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    

    using Nina.Configuration;

    public class ContactManagerConfiguration : HttpConfiguration
    {
        private readonly CompositionContainer container;

        public ContactManagerConfiguration(CompositionContainer container)
        {
            this.container = container;
            Configure.Views.WithNHaml(); // Would be nice to make this non-static and inject it.

            this.ResponseHandlers = RegisterResponseProcessorsForOperation;
        }
        
        public void RegisterRequestProcessorsForOperation(ICollection<HttpOperationHandler> handlers, HttpOperationDescription operation )
        {
            processors.Add(new JsonNetProcessor(operation, mode));
            processors.Add(new BsonProcessor(operation, mode));
            processors.Add(new FormUrlEncodedProcessor(operation, mode));
        }

        public void RegisterResponseProcessorsForOperation(HttpOperationDescription operation, IList<Processor> processors, MediaTypeProcessorMode mode)
        {
            processors.Add(new JsonNetProcessor(operation, mode));
            processors.Add(new BsonProcessor(operation, mode));
            processors.Add(new PngProcessor(operation, mode));
            // TODO: How would you set this up to work for any type?
            processors.Add(new ViewEngineProcessor<Contact>(operation, mode, "Templates/"));
        }

        // Get the instance from MEF
        public object GetInstance(Type serviceType, InstanceContext instanceContext, Message message)
        {
            var contract = AttributedModelServices.GetContractName(serviceType);
            var identity = AttributedModelServices.GetTypeIdentity(serviceType);

            // force non-shared so that every service doesn't need to have a [PartCreationPolicy] attribute.
            var definition = new ContractBasedImportDefinition(contract, identity, null, ImportCardinality.ExactlyOne, false, false, CreationPolicy.NonShared);
            return this.container.GetExports(definition).First().Value;
        }

        public void ReleaseInstance(InstanceContext instanceContext, object service)
        {
            // no op
        }
    }
}