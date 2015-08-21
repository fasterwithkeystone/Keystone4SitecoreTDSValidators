using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HedgehogDevelopment.SitecoreProject.Tasks;
using HedgehogDevelopment.SitecoreProject.Tasks.ProjectAnalysis;


namespace Keystone.BestPractices.TDS.Validators.Layout.Sublayouts
{
    [Validator("KEY010", Status.Error, Description = "When the datasource and paramater template is set with caching enabled, varybyparm is required.", Name = "Sublayouts: Vary Cache by Parm required")]
    public class CacheVaryByParamRequired : UserConfigurableValidator
    {
        public override IEnumerable<Problem> Validate(Dictionary<Guid, SitecoreDeployInfo> projectItems, XDocument scprojDocument)
        {
            //loop through each item in the TDS project
            foreach (var item in projectItems)
            {
                //check that the item path starts with a value specified in the Additional Properties list
                //otherwise we just ignore the item
                if (Settings.Properties.Any(
                        x => item.Value.Item.SitecoreItemPath.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                {

                    var cacheable = item.Value.ParsedItem.Fields["Cacheable"];
                    var parameterstemplate = item.Value.ParsedItem.Fields["Parameters Template"];
                    var varyby = item.Value.ParsedItem.Fields["VaryByParm"];
                    var datasourcelocation = item.Value.ParsedItem.Fields["Datasource Location"];
                    
                    if (!string.IsNullOrEmpty(datasourcelocation))
                    {
                        if (!string.IsNullOrEmpty(cacheable) && !string.IsNullOrEmpty(parameterstemplate) && string.IsNullOrEmpty(varyby))
                        {
                            //when a problem is found get the position of it in the TDS project file
                            ProblemLocation position = GetItemPosition(scprojDocument, item.Value.Item);

                            //create a report which will be displayed in the Visual Studio Error List
                            Problem report = new Problem(this, position)
                            {
                                Message =
                                    string.Format("A datasource driven sublayout which is cachable and has a parameter template assigned, must vary by parameter: {0}",
                                        item.Value.ParsedItem.Path)
                            };

                            yield return report;
                        }
                    }




                }
            }
        }

        public override ValidatorSettings GetDefaultSettings()
        {
            return new ValidatorSettings()
            {
                Properties = new List<string>()
                {
                    "/sitecore/layout/Sublayouts"
                }
            };
        }
    }
}
