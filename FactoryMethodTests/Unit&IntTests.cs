using server;


namespace FactoryMethodTests
{
    public class FactoryMethodTest
    {
       
        //Unit test to validate grid alert has been returned in factory method pattern
        [Test]
        public void AlertCreation_GridAlertReturned()
        {
            //ACT
            GridAlert alert = GridAlertFactory.CreateAlert();
            

            // ASSERT
            Assert.That(alert, Is.Not.Null);
            Assert.That(alert, Is.InstanceOf<GridAlert>());
        }

        //Integration test used to validate factory pattern return valid message and alert

        [Test]
        public void CreatedAlert_ReturnsValidMessage()
        {  
            //ACT
            GridAlert alert = GridAlertFactory.CreateAlert();
            string message = alert.GetMessage();

            //
            Assert.That(alert, Is.Not.Null);
            Assert.That(message, Is.Not.Null);
            Assert.That(message, Is.Not.Empty);

        }
    }
}
