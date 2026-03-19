using SecureBunkerCore;
using SecureBunkerCore.Data;

namespace SecureBunker.UnitTests
{
    [TestFixture]
    internal class EncryptDocumentTest
    {
        [SetUp]
        public void Setup()
        {

            //create mock document
            FileManipulation.ListItems.Clear();
            
            DataItems Item1 = new DataItems() { Description= "Description 1", Name = "Item 1", OtherText = "Other Text 1", Password = "Password 1", URL = "http://url1.com", User = "User 1" };
            FileManipulation.ListItems.Add(Item1);

            DataItems Item2 = new DataItems() { Description = "Description 2", Name = "Item 2", OtherText = "Other Text 2", Password = "Password 2", URL = "http://url2.com", User = "User 2" };
            FileManipulation.ListItems.Add(Item2);
            
            DataItems Item3 = new DataItems() { Description = "Description 3", Name = "Item 3", OtherText = "Other Text 3", Password = "Password 3", URL = "http://url3.com", User = "User 3" };
            FileManipulation.ListItems.Add(Item3);

        }

        [Test]
        public void Encrypt_Decript_Items()
        {
            string userLogin = "martí";
            string passwordLogin = "1AaTg@434";    
            // Arrange
            var originalList = new List<DataItems>(FileManipulation.ListItems);

            // Create the encrypted data
            var encryptedStream = FileManipulation.EncryptDocument(userLogin, passwordLogin);

            // Act
            // Clear list to simulate fresh load
            FileManipulation.ListItems.Clear();

            //decript and load document
            FileManipulation.LoadDocument(encryptedStream, userLogin, passwordLogin);
           
            Assert.That(FileManipulation.ListItems, Has.Exactly(3).Items, "There should be exactly 3 items loaded.");            
        }
    }
}
