using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public class MallocMessageTests : jemallocTest
    {
        
        [Fact]
        public void CanSetMallocMessageAction()
        {
            StringBuilder messagesBuilder = new StringBuilder();
            Je.MallocMessage += (m) => { messagesBuilder.Append(m); };
            Je.MallocStatsPrint();
            string messages = messagesBuilder.ToString();
            Assert.True(messages.Contains("opt.narenas: 3"));
        }
    }
}
