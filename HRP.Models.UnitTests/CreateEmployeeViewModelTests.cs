namespace HRP.Models.UnitTests
{
    using HRP.Models.Employee;
    using Xunit;

    public class CreateEmployeeViewModelTests
    {
        [Fact]
        public void ProfilePicLink_ReturnDefaultlink_WhenProfilePicIsNullOrEmpty()
        {
            var model = new CreateEmployeeViewModel
            {
                ProfilePic = string.Empty
            };

            Assert.Equal("/assets/styles/images/profile.png", model.ProfilePicLink);
        }

        [Fact]
        public void ProfilePicLink_ReturnExpectedLink_WhenProfilePicIsNotNullAndEmpty()
        {
            var model = new CreateEmployeeViewModel
            {
                ProfilePic = "abc.png"
            };

            Assert.Equal("/ProfilePicture/abc.png", model.ProfilePicLink);
        }
    }
}
