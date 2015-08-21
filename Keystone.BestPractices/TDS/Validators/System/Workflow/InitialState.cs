using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HedgehogDevelopment.SitecoreProject.Tasks;
using HedgehogDevelopment.SitecoreProject.Tasks.ProjectAnalysis;

namespace Keystone.BestPractices.TDS.Validators.System.Workflow
{
    [Validator("KEY002", Status.Error, Description = "Workflow: All workflows must have an initial state assigned.", Name = "Workflow initial state check")]
    public class InitialState : UserConfigurableValidator
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

                    
                    
                    var fld = item.Value.ParsedItem.Fields["Initial State"];


                    if (fld != null && fld == String.Empty)
                    {
                        //when a problem is found get the position of it in the TDS project file
                        ProblemLocation position = GetItemPosition(scprojDocument, item.Value.Item);

                        //create a report which will be displayed in the Visual Studio Error List
                        Problem report = new Problem(this, position)
                        {
                            Message = string.Format("This workflow is missing an intial state: {0}", item.Value.ParsedItem.Path)
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
                    "/sitecore/system/workflows",
                }
            };
        }
    }
}