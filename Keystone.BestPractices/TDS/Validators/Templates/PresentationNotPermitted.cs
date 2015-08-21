using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HedgehogDevelopment.SitecoreProject.Tasks;
using HedgehogDevelopment.SitecoreProject.Tasks.ProjectAnalysis;

namespace Keystone.BestPractices.TDS.Validators.Templates
{
    [ValidatorAttribute("KEY001", Status.Error, Description = "Templates: Templates in this folder cannot have presentation assigned.", Name = "Templates: No Presentation")]
    public class PresentationNotPermitted : UserConfigurableValidator
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

                    
                    var fld = item.Value.ParsedItem.Fields["__Renderings"];
                    

                    if (!string.IsNullOrEmpty(fld))
                    {
                        //when a problem is found get the position of it in the TDS project file
                        ProblemLocation position = GetItemPosition(scprojDocument, item.Value.Item);

                        //create a report which will be displayed in the Visual Studio Error List
                        Problem report = new Problem(this, position)
                        {
                            Message = string.Format("This template should not have presentation set: {0}", item.Value.ParsedItem.Path)
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
                    "/sitecore/templates/keystone/base/",
                }
            };
        }
    }
}