// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Server.Kestrel.Http;

namespace Microsoft.AspNetCore.Server.KestrelTests
{
    /// <summary>
    /// Summary description for TestServer
    /// </summary>
    public class TestServer : IDisposable
    {
        private LibuvEngine _engine;
        private IDisposable _server;
        private ServerAddress _address;

        public TestServer(RequestDelegate app)
            : this(app, new TestServiceContext())
        {
        }

        public TestServer(RequestDelegate app, TestServiceContext context)
            : this(app, context, "http://127.0.0.1:0/")
        {
        }

        public TestServer(RequestDelegate app, TestServiceContext context, string serverAddress)
        {
            Context = context;

            //context.FrameFactory = (connectionContext, serviceContext) =>
            //{
            //    return new Frame<HttpContext>(new DummyApplication(app), connectionContext, serviceContext);
            //};

            try
            {
                _engine = new LibuvEngine(context);
                _engine.Start(1);
                _address = ServerAddress.FromUrl(serverAddress);
                _server = _engine.CreateServer(new ListenerContext
                {
                    Address = _address
                });
            }
            catch
            {
                _server?.Dispose();
                _engine?.Dispose();
                throw;
            }
        }

        public int Port => _address.Port;

        public TestServiceContext Context { get; }

        public TestConnection CreateConnection()
        {
            return new TestConnection(this);
        }

        public void Dispose()
        {
            _server.Dispose();
            _engine.Dispose();
        }
    }
}