﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Data;
using etl.lib.util;
using etl.lib.extractor;
using etl.lib.transformer;
using etl.lib.loader;

namespace etl.lib.control
{
    public class Controller 
    {
        Arguments arguments = null;

        List<System.Reflection.Assembly> assemblies = null; 

        public void execute(Arguments arg)
        {
            IExtractor extractor;
            ITransformer transformer;
            ILoader loader;

            init(arg);

            loadAssemblies();

            createClasses(out extractor, out transformer, out loader);

            run(arg, extractor, transformer, loader);
        }

        private void init(Arguments arg)
        {
            this.arguments = arg;
        }

        private void createClasses(out IExtractor extractor, out ITransformer transformer, out ILoader loader)
        {
            extractor = getExtractor();
            transformer = getTransformer();
            loader = getLoader();
        }

        private void loadAssemblies()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;

            assemblies = new List<System.Reflection.Assembly>();

            string[] dlls = Directory.GetFiles(appPath, "*.dll");

            foreach( string dll in dlls)
            {
                try
                {
                    Assembly assy = Assembly.LoadFile(dll);
                    assemblies.Add(assy);
                }
                catch(Exception x)
                {
                    Logger.info("Not loading " + dll + ". " + x.Message);
                }

            }
        }

        protected void run(Arguments arg, IExtractor extractor, ITransformer transformer, ILoader loader)
        {
            DataTable data = null;

            Logger.info(extractor.GetType().ToString() + " Extraction Start");
            extractor.setArgs(arg);
            data = extractor.extract();

            Logger.info(transformer.GetType().ToString() + " Transformation Start");
            transformer.setArgs(arg);
            data = transformer.transform( data);

            Logger.info(loader.GetType().ToString() + " Loading Start");
            loader.setArgs(arg);
            loader.load(data);
            Logger.info(loader.GetType().ToString() + " Loading Complete");
        }

        private IExtractor getExtractor()
        {

            IExtractor extractor = (IExtractor) createObject(ExtractorClass);
            return extractor;
        }

        public string ExtractorClass
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public string TransformerClass
        {
            get
            {
                string transformerClass = arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
                if(string.IsNullOrEmpty(transformerClass))
                {
                    transformerClass = "etl.lib.transformer.DefaultTransformer";
                }
                return transformerClass;
            }
        }

        public string LoaderClass
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        private  object createObject(string className)
        {
            object o = null;
            bool found = false;

            for (int i = 0; (i < assemblies.Count) && (!found); i++)
            {
                try
                {
                    Assembly assy = assemblies[i];
                    AssemblyName assyName = assy.GetName();
                    var inst = Activator.CreateInstance( assyName.Name, className);
                    o = inst.Unwrap();
                    found = true;
                }   
                catch(Exception x)
                {
                    Logger.info(x.Message);
                }
            }

            if (!found) throw new EtlException("Class cannot be instantiated: " + className);

            return o;
        }

        private ITransformer getTransformer()
        {
            ITransformer xformer = (ITransformer)createObject(TransformerClass);
            return xformer;
        }
       

        private ILoader getLoader()
        {
            ILoader loader = (ILoader)createObject(LoaderClass);
            return loader;
        }
    }
}
