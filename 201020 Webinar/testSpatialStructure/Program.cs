using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeometryGym.Ifc;

namespace InfraDeploymentHackathon
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			DatabaseIfc db = new DatabaseIfc(ReleaseVersion.IFC4X3);

			IfcRailway sydneyLightRail = new IfcRailway(db);
			sydneyLightRail.Name = "Sydney Light Rail";
			sydneyLightRail.ObjectType = "Light Rail Complexes";

			IfcProject project = new IfcProject(sydneyLightRail, "", IfcUnitAssignment.Length.Metre);

			IfcClassification uniclass2015 = new IfcClassification(db, "Uniclass2015");
			IfcClassificationReference lightRailComplexes = new IfcClassificationReference(uniclass2015);
			lightRailComplexes.Identification = "Co_80_50_45";
			lightRailComplexes.Name = "Light Rail Complexes";

			lightRailComplexes.Associate(sydneyLightRail);

			IfcFacilityPartTypeSelect facilityPartTypeSelect = new IfcFacilityPartTypeSelect(IfcRailwayPartTypeEnum.USERDEFINED);
			IfcFacilityPart cbdCorridor = new IfcFacilityPart(sydneyLightRail, "CBD and SE Light Rail", facilityPartTypeSelect, IfcFacilityUsageEnum.LONGITUDINAL);
			new IfcClassificationReference(uniclass2015) { Identification = "En_80_50_71", Name = "Railway Corridors" }.Associate(cbdCorridor);

			IfcSpace deStop = new IfcSpace(cbdCorridor, "DE Stop");
			deStop.PredefinedType = IfcSpaceTypeEnum.USERDEFINED;
			deStop.ObjectType = "Light Rail Stops";

			new IfcClassificationReference(uniclass2015) { Identification = "SL_80_50_47", Name = "Light Rail Stops" }.Associate(deStop);

			IfcSpace platform1_2 = new IfcSpace(deStop, "Platform 1/2");

			IfcClassificationReference classificationTicketSystem = new IfcClassificationReference(uniclass2015) { Identification = "Ss_75_90_80", Name = "Ticketing Systems" };
			IfcSystem opal = new IfcSystem(sydneyLightRail, "Opal system");
			classificationTicketSystem.Associate(opal);

			IfcDistributionSystem platform1_2TicketSystem = new IfcDistributionSystem(platform1_2, "DE Stop Platform 1/2 Ticket System", IfcDistributionSystemEnum.USERDEFINED);
			opal.AddAggregated(platform1_2TicketSystem);
			classificationTicketSystem.Associate(platform1_2TicketSystem);

			IfcCommunicationsApplianceType opalTicketMachineType = new IfcCommunicationsApplianceType(db, "Opal Ticket Machine", IfcCommunicationsApplianceTypeEnum.USERDEFINED);
			new IfcClassificationReference(uniclass2015) { Identification = "Pr_75_75_27_80", Name = "Smart card readers and writers" }.Associate(opalTicketMachineType);
			project.AddDeclared(opalTicketMachineType);

			IfcLocalPlacement ticketMachineLocalPlacement = null;
			IfcProductDefinitionShape ticketMachineShape = null;
			IfcCommunicationsAppliance ticketMachine = new IfcCommunicationsAppliance(platform1_2, ticketMachineLocalPlacement, ticketMachineShape, platform1_2TicketSystem);

			IfcPropertySingleValue dateCommissioned = new IfcPropertySingleValue(db, "Date Commissioned", new IfcDate(DateTime.Now));
			new IfcPropertySet("TfNSW_Asset", dateCommissioned);

			System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			saveFileDialog.Filter = "IFC BIM Data (*.ifc,*.ifcxml,*.ifcjson,*.ifczip)|*.ifc;*.ifcxml;*.ifcjson;*.ifczip"; ;
			if(saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				db.WriteFile(saveFileDialog.FileName);
		}
	}
}
