using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeometryGym.Ifc;

namespace testBoreHole
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			//Sample Data https://www.tmr.qld.gov.au/-/media/busind/techstdpubs/Geotechnical/Geotech-Borehole-Logging/Geotechnical-Borehole-Logging.pdf?la=en
			//http://creativecommons.org/licenses/by/3.0/au/
			// © State of Queensland(Department of Transport and Main Roads) 2016
			
			// Suggested enhancements to test :
			//		Nominate Status and Reference Number
			//		Add Geolocation
			//		Use linear placement for statum layers
			//		Capture Auger, Casing, Wash Boring and Core drilling extents
			//		Add stratum data Impact Strength, Defect Spacing, Additional Data and Test Results, Sample Tests

			DatabaseIfc db = new DatabaseIfc(ReleaseVersion.IFC4X3);
			db.Factory.Options.AngleUnitsInRadians = false;

			IfcSite site = new IfcSite(db, "Coombabah Creek Abutment");
			IfcProject project = new IfcProject(site, "FG6280", IfcUnitAssignment.Length.Metre) { LongName = "Gold Coast Light Rail(GCLR) Stage 2" };
			project.Description = "http://creativecommons.org/licenses/by/3.0/au/ © State of Queensland(Department of Transport and Main Roads) 2016";

			IfcAxis2Placement3D placement = new IfcAxis2Placement3D(new IfcCartesianPoint(db, 0, 0, 0.99));
			IfcLocalPlacement boreHolePlacement = new IfcLocalPlacement(site.ObjectPlacement, placement);
			IfcBorehole borehole = new IfcBorehole(site, boreHolePlacement, null);
			borehole.Name = "BH-CC-01";
			borehole.Description = "Sample borehole data published at  https://www.tmr.qld.gov.au/-/media/busind/techstdpubs/Geotechnical/Geotech-Borehole-Logging/Geotechnical-Borehole-Logging.pdf?la=en";

			Pset_BoreholeCommon pset_BoreholeCommon = new Pset_BoreholeCommon(borehole);
			pset_BoreholeCommon.GroundwaterDepth = -5; //Not identified in borehole log
			//new Pset_Uncertainty(borehole) { Basis = PEnum_UncertaintyBasis.MEASUREMENT }; 
			Pset_GeotechnicalAssemblyCommon psetGeotechnicalAssemblyCommon = new Pset_GeotechnicalAssemblyCommon(borehole);
			psetGeotechnicalAssemblyCommon.Purpose = PEnum_StrataAssemblyPurpose.GEOLOGICAL;

			//Should some of these properties be standard?
			//Should actors such as drilling company be IfcActor etc

			List<IfcProperty> tmrBoreholeProperties = new List<IfcProperty>();
			tmrBoreholeProperties.Add(new IfcPropertySingleValue(db, "Driller", new IfcIdentifier("North Coast Drilling")));
			tmrBoreholeProperties.Add(new IfcPropertySingleValue(db, "LoggedBy", new IfcIdentifier("D.Colborne/J.Armstrong")));
			tmrBoreholeProperties.Add(new IfcPropertySingleValue(db, "ReviewedBy", new IfcIdentifier("S.Foley")));
			tmrBoreholeProperties.Add(new IfcPropertySingleValue(db, "Plunge", new IfcPlaneAngleMeasure(90)));
			tmrBoreholeProperties.Add(new IfcPropertySingleValue(db, "Date Started", new IfcDate(new DateTime(2015, 09, 10))));
			tmrBoreholeProperties.Add(new IfcPropertySingleValue(db, "Date Completed", new IfcDate(new DateTime(2015, 09, 11))));
			tmrBoreholeProperties.Add(new IfcPropertySingleValue(db, "Reference No", new IfcIdentifier("H12327")));
			tmrBoreholeProperties.Add(new IfcPropertySingleValue(db, "Job No", new IfcIdentifier("498/04375")));
			new IfcPropertySet(borehole, "TMR_Borehole", tmrBoreholeProperties);

			IfcCircleProfileDef boreProfile = new IfcCircleProfileDef(db, "BoreHole", 0.05);

			IfcMaterial siltyClayTopsoil = new IfcMaterial(db, "Silty Clay (TopSoil)");
			siltyClayTopsoil.Description = "Dark grey, moist.";

			placement = new IfcAxis2Placement3D(new IfcCartesianPoint(db, 0, 0, -0.1));
			IfcLocalPlacement localPlacement = new IfcLocalPlacement(boreHolePlacement, placement); 
			IfcExtrudedAreaSolid extrudedAreaSolid = new IfcExtrudedAreaSolid(boreProfile, 0.1);
			IfcProductDefinitionShape shape = new IfcProductDefinitionShape(new IfcShapeRepresentation(extrudedAreaSolid));

			//gg Toolkit uses nesting rather than aggregating for stratums
			IfcSolidStratum solidStratum = new IfcSolidStratum(borehole, localPlacement, shape); 
			solidStratum.SetMaterial(siltyClayTopsoil);

			IfcClassification soilClassification = new IfcClassification(db, "Soil Descriptions");
			soilClassification.Description = "Description and Classification of Soils for Geotechnical Purposes: Refer to AS1726 - 1993(Appendix A).";

			IfcMaterial siltyClayEstuarine = new IfcMaterial(db, "Silty CLAY (Estuarine)");
			siltyClayEstuarine.Description = "Dark grey, moist, soft.";
			placement = new IfcAxis2Placement3D(new IfcCartesianPoint(db, 0, 0, -1.2));
			localPlacement = new IfcLocalPlacement(boreHolePlacement, placement);
			extrudedAreaSolid = new IfcExtrudedAreaSolid(boreProfile, 1.1);
			shape = new IfcProductDefinitionShape(new IfcShapeRepresentation(extrudedAreaSolid));
			solidStratum = new IfcSolidStratum(borehole, localPlacement, shape);
			solidStratum.SetMaterial(siltyClayEstuarine);
			IfcClassificationReference soilClassificationCI = new IfcClassificationReference(soilClassification)
			{
				Identification = "CI",
				Description = "Inorganic clays of low to medium plasticity, gravelly clays, sandy clays, silty clays, lean clays"
			};
			soilClassificationCI.Associate(solidStratum);
#warning Should soil classification be assigned to soil or to stratum?

			IfcMaterial clayeySANDEstuarine = new IfcMaterial(db, "Clayey SAND (Estuarine)");
			clayeySANDEstuarine.Description = "Dark grey, wet, very loose.";
			placement = new IfcAxis2Placement3D(new IfcCartesianPoint(db, 0, 0, -6.4));
			localPlacement = new IfcLocalPlacement(boreHolePlacement, placement);
			extrudedAreaSolid = new IfcExtrudedAreaSolid(boreProfile, 5.2);
			shape = new IfcProductDefinitionShape(new IfcShapeRepresentation(extrudedAreaSolid));
			solidStratum = new IfcSolidStratum(borehole, localPlacement, shape);
			solidStratum.SetMaterial(clayeySANDEstuarine);
			solidStratum.Description = "Fine to medium grained. Shell fragments throughout.";

			IfcClassificationReference soilClassificationSC = new IfcClassificationReference(soilClassification)
			{
				Identification = "SC",
				Description = "Clayey sands, sandclay mixtures"
			};
			soilClassificationSC.Associate(solidStratum);

			IfcMaterial METAGREYWACKEdcf = new IfcMaterial(db, "METAGREYWACKE (DCf)");
			
			placement = new IfcAxis2Placement3D(new IfcCartesianPoint(db, 0, 0, -6.8));
			localPlacement = new IfcLocalPlacement(boreHolePlacement, placement);
			extrudedAreaSolid = new IfcExtrudedAreaSolid(boreProfile, 0.4);
			shape = new IfcProductDefinitionShape(new IfcShapeRepresentation(extrudedAreaSolid));
			solidStratum = new IfcSolidStratum(borehole, localPlacement, shape);
			solidStratum.SetMaterial(METAGREYWACKEdcf);
			solidStratum.Description = "XW: Recovered as orange brown to grey, moist, very stiff, Silty CLAY.";

			IfcClassification rockClassification = new IfcClassification(db, "Rock Descriptions");
			rockClassification.Description = "Refer to AS1726-1993 (Appendix A3.3) for the description and classification of rock material composition";


			IfcClassificationReference rockClassificationXW = new IfcClassificationReference(rockClassification)
			{
				Identification = "XW",
				Description = "Rock is weathered to such an extent that it has 'soil' properties, i.e. it either disintegrates or can be remoulded in water, but substance fabric and rock structure still recognisable."
			};
			rockClassificationXW.Associate(solidStratum);

			placement = new IfcAxis2Placement3D(new IfcCartesianPoint(db, 0, 0, -12.7));
			localPlacement = new IfcLocalPlacement(boreHolePlacement, placement);
			extrudedAreaSolid = new IfcExtrudedAreaSolid(boreProfile, 5.9);
			shape = new IfcProductDefinitionShape(new IfcShapeRepresentation(extrudedAreaSolid));
			solidStratum = new IfcSolidStratum(borehole, localPlacement, shape);
			solidStratum.SetMaterial(METAGREYWACKEdcf);
			solidStratum.Description = "SW: Dark grey, fine to medium grained, indistinctly foliated, high to very high strength.";
			IfcClassificationReference rockClassificationSW = new IfcClassificationReference(rockClassification)
			{
				Identification = "SW",
				Description = "Rock is slightly discoloured but shows little or no change of strength from fresh rock."
			};
			rockClassificationSW.Associate(solidStratum);


			System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			saveFileDialog.Filter = "IFC BIM Data (*.ifc,*.ifcxml,*.ifcjson,*.ifczip)|*.ifc;*.ifcxml;*.ifcjson;*.ifczip"; ;
			if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				db.WriteFile(saveFileDialog.FileName);
		}
	}
}
