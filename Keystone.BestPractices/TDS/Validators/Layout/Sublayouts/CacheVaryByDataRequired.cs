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
    [Validator("KEY008", Status.Error, Description = "When the datasource is set and caching is enabled, varybydata is required.", Name = "Sublayouts: Vary Cache by Datasource required")]
    public class CacheVaryByDataRequired : UserConfigurableValidator
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
                    var varyby = item.Value.ParsedItem.Fields["VaryByData"];
                    var datasourcelocation = item.Value.ParsedItem.Fields["Datasource Location"];

                    if (!string.IsNullOrEmpty(datasourcelocation) && !string.IsNullOrEmpty(cacheable) && string.IsNullOrEmpty(varyby))
                    {

                        //when a problem is found get the position of it in the TDS project file
                        ProblemLocation position = GetItemPosition(scprojDocument, item.Value.Item);

                        //create a report which will be displayed in the Visual Studio Error List
                        Problem report = new Problem(this, position)
                        {
                            Message =
                                string.Format("1: A datasource driven sublayout which is cachable must vary by data: {0}",
                                    item.Value.ParsedItem.Path)
                        };

                        yield return report;

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
