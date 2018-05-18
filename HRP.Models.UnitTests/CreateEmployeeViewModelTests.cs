namespace HRP.Models.UnitTests
{
    using HRP.Models.Employee;
    using Xunit;

    public class CreateEmployeeViewModelTests
    {
        [Fact]
        public void ProfilePicLink_WhenProfilePicIsNullOrEmpty_ReturnDefaultlink()
        {
            var model = new CreateEmployeeViewModel
            {
                ProfilePic = string.Empty
            };

            Assert.Equal("/assets/styles/images/profile.png", model.ProfilePicLink);
        }

        [Fact]
        public void ProfilePicLink_WhenProfilePicIsNotNullAndEmpty_ReturnExpectedLink()
        {
            var model = new CreateEmployeeViewModel
            {
                ProfilePic = "abc.png"
            };

            Assert.Equal("/ProfilePicture/abc.png", model.ProfilePicLink);
        }
    }
}
