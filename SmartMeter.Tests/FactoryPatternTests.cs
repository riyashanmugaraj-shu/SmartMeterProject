//https://www.c-sharpcorner.com/article/mastering-the-factory-pattern-in-c-sharp-13-best-practices-with-real-world-examples
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
            //ensures alert is not empty and instance is created
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

            //ASSERT
            //ensures the alert and message is not null or empty
            Assert.That(alert, Is.Not.Null);
            Assert.That(message, Is.Not.Null);
            Assert.That(message, Is.Not.Empty);

        }

      
      
            
               
                
            
            
        
    }
}
