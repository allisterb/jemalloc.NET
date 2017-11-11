using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;

namespace jemalloc.Bindings
{
    public class JemallocLibrary : ILibrary
    {
        /// Setup the driver options here.
        public void Setup(Driver driver)
        {
            DriverOptions options = driver.Options;
            options.GeneratorKind = GeneratorKind.CSharp;
            Module module = options.AddModule("jemalloc");
            module.Defines.AddRange(@"JEMALLOC_NO_PRIVATE_NAMESPACE;REENTRANT;WINDLL;DLLEXPORT;JEMALLOC_DEBUG;DEBUG".Split(';'));
            module.IncludeDirs.Add(@"..\..\..\..\jemalloc\include\jemalloc");
            module.IncludeDirs.Add(@"..\..\..\..\jemalloc\include\jemalloc\internal");
            module.IncludeDirs.Add(@"..\..\..\..\jemalloc\include\msvc_compat");
            module.IncludeDirs.Add(@".\");
            module.Headers.Add("jemalloc-cs.h");
            
            module.LibraryDirs.Add(@"..\..\..\..\x64\Debug\");
            //module.Libraries.Add("jemallocd.lib");
        }

        /// Setup your passes here.
        public void SetupPasses(Driver driver)
        {
           
        }

        /// Do transformations that should happen before passes are processed.
        public void Preprocess(Driver driver, ASTContext ctx)
        {

        }

        /// Do transformations that should happen after passes are processed.
        public void Postprocess(Driver driver, ASTContext ctx)
        {

        }
    }
}
