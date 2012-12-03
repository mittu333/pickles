﻿#region License

/*
    Copyright [2011] [Jeffrey Cameron]

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

#endregion

using log4net;
using NGenerics.DataStructures.Trees;
using PicklesDoc.Pickles.DirectoryCrawler;
using PicklesDoc.Pickles.DocumentationBuilders.JSON;
using PicklesDoc.Pickles.TestFrameworks;
using System.Reflection;

namespace PicklesDoc.Pickles.DocumentationBuilders.DHTML
{
    public class DhtmlDocumentationBuilder : IDocumentationBuilder
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Configuration configuration;
        private readonly ITestResults testResults;

        public DhtmlDocumentationBuilder(Configuration configuration, ITestResults testResults)
        {
            this.configuration = configuration;
            this.testResults = testResults;
        }

        #region IDocumentationBuilder Members

        public void Build(GeneralTree<IDirectoryTreeNode> features)
        {
            if (log.IsInfoEnabled)
            {
                log.InfoFormat("Writing DHTML files to {0}", this.configuration.OutputFolder.FullName);
            }

            var resource = new DhtmlResourceSet(configuration);

            log.Info("DeployZippedDhtmlResourcesForExtraction");
            DeployZippedDhtmlResourcesForExtraction(resource);

            log.Info("UnzipDhtmlResources");
            UnzipDhtmlResources(resource);

            log.Info("UtilizeJsonBuilderToDumpJsonFeatureFileNextToDthmlResources");
            UtilizeJsonBuilderToDumpJsonFeatureFileNextToDthmlResources(features);
        }

        private void UtilizeJsonBuilderToDumpJsonFeatureFileNextToDthmlResources(GeneralTree<IDirectoryTreeNode> features)
        {
            var shit = new JSONDocumentationBuilder(configuration, testResults);
            shit.Build(features);
        }

        private void UnzipDhtmlResources(DhtmlResourceSet dhtmlResourceSet)
        {
            var unzipper = new UnZipper();
            unzipper.UnZip(dhtmlResourceSet.ZippedResources.AbsolutePath, configuration.OutputFolder.FullName, "BaseDhtmlFiles");
        }

        private void DeployZippedDhtmlResourcesForExtraction(DhtmlResourceSet dhtmlResourceSet)
        {
            var resourceWriter = new DhtmlResourceWriter(configuration, dhtmlResourceSet);
            resourceWriter.WriteZippedResources();
        }

        #endregion
    }
}