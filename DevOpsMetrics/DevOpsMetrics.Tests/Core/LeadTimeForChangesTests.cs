using DevOpsMetrics.Core;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DevOpsMetrics.Tests.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("UnitTest")]
    [TestClass]
    public class LeadTimeForChangesTests
    {

        [TestMethod]
        public void LeadTimeForChangesSingleOneDayTest()
        {
            //Arrange
            LeadTimeForChanges metrics = new LeadTimeForChanges();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = new List<KeyValuePair<DateTime, TimeSpan>>
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(0,45,0))
            };

            //Act
            float result = metrics.ProcessLeadTimeForChanges(leadTimeForChangesList, pipelineName, numberOfDays);
            LeadTimeForChangesModel model = new LeadTimeForChangesModel
            {
                LeadTimeForChangesMetric = result,
                LeadTimeForChangesMetricDescription = metrics.GetLeadTimeForChangesRating(result),
                IsProjectView = true,
                ItemOrder = 1,
                RateLimitHit = true
            };

            //Assert
            Assert.IsTrue(model != null);
            Assert.AreEqual(0.75f, model.LeadTimeForChangesMetric);
            Assert.AreEqual("Elite", model.LeadTimeForChangesMetricDescription);
            Assert.AreEqual(0.75f, model.LeadTimeForChangesMetricDisplayMetric);
            Assert.AreEqual("hours", model.LeadTimeForChangesMetricDisplayUnit);
            Assert.AreEqual(true, model.IsProjectView);
            Assert.AreEqual(1, model.ItemOrder);
            Assert.AreEqual(true, model.RateLimitHit);
        }

        [TestMethod]
        public void LeadTimeForChangesNullOneDayTest()
        {
            //Arrange
            LeadTimeForChanges metrics = new LeadTimeForChanges();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 1;
            List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = null;

            //Act
            float result = metrics.ProcessLeadTimeForChanges(leadTimeForChangesList, pipelineName, numberOfDays);

            //Assert
            Assert.AreEqual(0f, result);
        }

        [TestMethod]
        public void LeadTimeForChangesFiveSevenDaysEliteTest()
        {
            //Arrange
            LeadTimeForChanges metrics = new LeadTimeForChanges();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = new List<KeyValuePair<DateTime, TimeSpan>>
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(2, 0, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-1),new TimeSpan(0, 45, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-2), new TimeSpan(1, 0, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-3),new TimeSpan(0, 40, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-4), new TimeSpan(0, 50, 0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-34),  new TimeSpan(12, 0, 0)) //this record should be out of range
            };

            //Act
            float result = metrics.ProcessLeadTimeForChanges(leadTimeForChangesList, pipelineName, numberOfDays);
            string rating = metrics.GetLeadTimeForChangesRating(result);

            //Assert
            Assert.AreEqual(1.05, Math.Round((double)result, 4));
            Assert.AreEqual("Elite", rating);
        }

        [TestMethod]
        public void LeadTimeForChangesFiveSevenDaysHighTest()
        {
            //Arrange
            LeadTimeForChanges metrics = new LeadTimeForChanges();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = new List<KeyValuePair<DateTime, TimeSpan>>
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(2, 0, 0,0,0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-34),  new TimeSpan(12, 0, 0)) //this record should be out of range
            };

            //Act
            float result = metrics.ProcessLeadTimeForChanges(leadTimeForChangesList, pipelineName, numberOfDays);
            string rating = metrics.GetLeadTimeForChangesRating(result);

            //Assert
            Assert.AreEqual(48.00, Math.Round((double)result, 4));
            Assert.AreEqual("High", rating);
        }

        [TestMethod]
        public void LeadTimeForChangesFiveSevenDaysMediumTest()
        {
            //Arrange
            LeadTimeForChanges metrics = new LeadTimeForChanges();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = new List<KeyValuePair<DateTime, TimeSpan>>
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(8, 0, 0,0,0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-34),  new TimeSpan(12, 0, 0)) //this record should be out of range
            };

            //Act
            float result = metrics.ProcessLeadTimeForChanges(leadTimeForChangesList, pipelineName, numberOfDays);
            string rating = metrics.GetLeadTimeForChangesRating(result);

            //Assert
            Assert.AreEqual((24*8), Math.Round((double)result, 4));
            Assert.AreEqual("Medium", rating);
        }

        [TestMethod]
        public void LeadTimeForChangesFiveSevenDaysLowTest()
        {
            //Arrange
            LeadTimeForChanges metrics = new LeadTimeForChanges();
            string pipelineName = "TestPipeline.CI";
            int numberOfDays = 7;
            List<KeyValuePair<DateTime, TimeSpan>> leadTimeForChangesList = new List<KeyValuePair<DateTime, TimeSpan>>
            {
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now, new TimeSpan(31, 0, 0,0,0)),
                new KeyValuePair<DateTime, TimeSpan>(DateTime.Now.AddDays(-34),  new TimeSpan(12, 0, 0)) //this record should be out of range
            };

            //Act
            float result = metrics.ProcessLeadTimeForChanges(leadTimeForChangesList, pipelineName, numberOfDays);
            string rating = metrics.GetLeadTimeForChangesRating(result);

            //Assert
            Assert.AreEqual((24 * 31), Math.Round((double)result, 4));
            Assert.AreEqual("Low", rating);
        }



        [TestMethod]
        public void LeadTimeForChangesFiveSevenDaysNoneTest()
        {
            //Arrange
            LeadTimeForChanges metrics = new LeadTimeForChanges();
            
            //Act
            float result = 0f;
            string rating = metrics.GetLeadTimeForChangesRating(result);

            //Assert
            Assert.AreEqual(0f, result);
            Assert.AreEqual("None", rating);
        }

    }
}
