using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HedgehogDevelopment.SitecoreProject.Tasks;
using HedgehogDevelopment.SitecoreProject.Tasks.ProjectAnalysis;

namespace Keystone.BestPractices.TDS.Validators.System.Workflow
{
    [Validator("KEY003", Status.Error, Description = "Workflow: All workflows must have at least one final state.", Name = "Workflow final state check")]
    public class FinalState : UserConfigurableValidator
    {
        public override IEnumerable<Problem> Validate(Dictionary<Guid, SitecoreDeployInfo> projectItems, XDocument scprojDocument)
        {

            var states = new List<SitecoreDeployInfo>();
            var workflows = new List<SitecoreDeployInfo>();

            //loop through each item in the TDS project
            foreach (var item in projectItems)
            {
                //check that the item path starts with a value specified in the Additional Properties list
                //otherwise we just ignore the item
                if (Settings.Properties.Any(
                        x => item.Value.Item.SitecoreItemPath.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                {

                    //get the name of the item from the Sitecore Item Path
                    //var name = item.Value.Item.SitecoreItemPath.Split('/').Last();

                    var intialStateField = item.Value.ParsedItem.Fields["Initial State"];
                    var finalStateField = item.Value.ParsedItem.Fields["Final"];


                    if (intialStateField != null)
                    {
                        workflows.Add(item.Value);
                    }
                    else if (finalStateField != null && finalStateField != String.Empty)
                    {
                        states.Add(item.Value);
                    }

                }
            }


            foreach (var workflow in workflows)
            {
                var finalstates = states.Where(x => x.Item.SitecoreItemPath.Contains(workflow.Item.SitecoreItemPath));

                if (!finalstates.Any())
                {
                    //when a problem is found get the position of it in the TDS project file
                    ProblemLocation position = GetItemPosition(scprojDocument, workflow.Item);

                    //create a report which will be displayed in the Visual Studio Error List
                    Problem report = new Problem(this, position)
                    {
                        Message =
                            string.Format("This workflow is missing a final state: {0}", workflow.ParsedItem.Path)
                    };

                    yield return report;
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