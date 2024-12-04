/*
Copyright 2024 Tamas Bolner

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Text;
using Xunit;
using RainbowLatinReader;

namespace unit_tests;

public class PageTests
{
    [Fact]
    public void TestPageProcessing1()
    {
        MockTemplateEngine engine = new();

        var page = new Page(new MockCanonLitDoc(), new MockLemmatizedDoc(),
            engine, "/etc/output");
        
        page.Process();

        Assert.True(page.GetLastError() == null, "Page processing failed with error: "
			+ (page.GetLastError() ?? new RainbowLatinException("?")).ToString());

        Assert.True(engine.GetDataForPath("/etc/output/phi1234.phi001_1.html") != null,
            "Page was not generated: /etc/output/phi1234.phi001_1.html");
    }
}
