using GIPractice.Application.Querying;
using GIPractice.Core.Entities;

namespace GIPractice.Tests;

[TestClass]
public sealed class QueryFieldTests
{
    [TestMethod]
    public void Canonical_patient_field_composes_through_endoscopy_relationship()
    {
        var patient = new Patient { FirstName = "Ada" };
        var encounter = new Encounter { Patient = patient };
        var endoscopy = new Endoscopy { Encounter = encounter };

        var selector = PatientFields.EndoscopyFirstName.Selector.Compile();

        Assert.AreEqual("patient.firstName", PatientFields.EndoscopyFirstName.Name);
        Assert.AreEqual("Ada", selector(endoscopy));
    }
}
