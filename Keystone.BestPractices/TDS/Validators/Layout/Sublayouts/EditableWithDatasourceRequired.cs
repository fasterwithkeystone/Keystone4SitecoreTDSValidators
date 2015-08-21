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
    [Validator("KEY009", Status.Error, Description = "When the datasource is set the sublayout must be editable.", Name = "Sublayouts: Editable sublayout")]
    public class EditableWithDatasourceRequired : UserConfigurableValidator
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

                    var editable = item.Value.ParsedItem.Fields["Editable"];
                    var datasourcelocation = item.Value.ParsedItem.Fields["Datasource Location"];

                    if (!string.IsNullOrEmpty(datasourcelocation) && editable != null && editable == String.Empty)
                    {

                        //when a problem is found get the position of it in the TDS project file
                        ProblemLocation position = GetItemPosition(scprojDocument, item.Value.Item);

                        //create a report which will be displayed in the Visual Studio Error List
                        Problem report = new Problem(this, position)
                        {
                            Message =
                                string.Format("A datasource driven sublayout must have it's 'Editable' setting checked: {0}",
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
