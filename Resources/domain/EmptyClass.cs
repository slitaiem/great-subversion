using System;
using System.Collections.Generic;
namespace AndroidBeacon
{
	public class Location
	{
		public int idLocation { get; set; }
		public int floor { get; set; }
		public int section { get; set; }
	}

	public class PublicService
	{
		public int idService { get; set; }
		public string serviceName { get; set; }
		public string serviceDescription { get; set; }
		public List<Location> locations { get; set; }
		public string textContent { get; set; }
		public string priority { get; set; }
		public object profiles { get; set; }
	}
	public class Bdp
	{
		public int idDevice { get; set; }
		public string manufacturer { get; set; }
		public Location location { get; set; }
		public string uuId { get; set; }
	}

	public class ServiceCategory
	{
		public int idServiceCategory { get; set; }
		public string categoryName { get; set; }
	}
	public class UserProfile
	{
		public int profileId { get; set; }
		public string profileName { get; set; }
		public List<object> users { get; set; }
		public List<PublicService> publicServices { get; set; }
		public bool @public { get; set; }
	}

	public class PrivateService
	{
		public int idService { get; set; }
		public string serviceName { get; set; }
		public string serviceDescription { get; set; }
		public List<Location> locations { get; set; }
		public string textContent { get; set; }
		public object users { get; set; }
		public string dataTypeSent { get; set; }
	}

	public class UserInfo
	{
		public string idUser { get; set; }
		public string userFirstName { get; set; }
		public string userLastName { get; set; }
		public long userBirthDate { get; set; }
		public List<UserProfile> userProfiles { get; set; }
		public List<PrivateService> privateServices { get; set; }
		public string userEmail { get; set; }
		public List<Bdp> bdps { get; set; }
		public List<ServiceCategory> serviceCategories { get; set; }

	}
}

