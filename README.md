# Keystone4SitecoreTDSValidators
Keystone for Sitecore is a development accelerator for Sitecore. This project is a collection of custom Team Development for Sitecore (TDS) validators we use in our product development.

For more information on Keystone for Sitecore please see our website http://www.fasterwithkeystone.com

1. Download the solution.
2. Update the references for your version of TDS
3. Build the solution
3. Deploy the resulting DLLs to your TDS directory (C:\Program Files (x86)\MSBuild\HedgehogDevelopment\SitecoreProject\v9.0.)

The sample validators in this solution include:

- cache settings on sublayouts/renderings
- requires a datasource location
- requires a datasource template
- workflow has an initial and final state
- template has no presentation details
- template has presentation details
- template has no direct fields (want to force inheritance)
