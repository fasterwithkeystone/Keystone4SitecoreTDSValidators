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
    [Validator("KEY006", Status.Error, Description = "Sublayout Datasource Template field is required.", Name = "Sublayouts: Datasource Template required")]
    public class DatasourceTemplateRequired : UserConfigurableValidator
    {

        public override IEnumerable<Problem> Validate(Dictionary<Guid, SitecoreDeployInfo> projectItems,
            XDocument scprojDocument)
        {
            //loop through each item in the TDS project
            foreach (var item in projectItems)
            {
                //check that the item path starts with a value specified in the Additional Properties list
                //otherwise we just ignore the item
                if (Settings.Properties.Any(
                    x => item.Value.Item.SitecoreItemPath.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                {

                    var fld = item.Value.ParsedItem.Fields["Datasource Template"];

                    if (String.IsNullOrEmpty(fld) && !TemplateNamesToIgnore().Contains(item.Value.Item.TemplateName))
                    {
                        //when a problem is found get the position of it in the TDS project file
                        ProblemLocation position = GetItemPosition(scprojDocument, item.Value.Item);

                        //create a report which will be displayed in the Visual Studio Error List
                        Problem report = new Problem(this, position)
                        {
                            Message =
                                string.Format("The 'Datasource Template' is required: {0}",
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
                    "/sitecore/layout/Sublayouts/Site/Components/"
                }
            };
        }

        public List<string> TemplateNamesToIgnore()
        {
            return new List<string>()
            {
                "sublayout folder"
            };
        }
    }
}
