
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Java.Util;
using System.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
namespace AndroidBeacon
{
	
	[assembly: Application( Theme = "@android:style/Theme.Holo.Light")]
	[Activity (Label = "LoginActivity", MainLauncher = true)]			
	public class LoginActivity : Activity 
	{

		private Button btnSignIn;
		private EditText editLogin, editPassword;
		private ISharedPreferences loginPrefs;
		private ISharedPreferencesEditor loginEditor;
		private string login, password, login1, password1;
		private Intent loginIntent;
		private List<String> accounts = new List<String>();
		private List<String> services = new List<String>();
		private List<String> devicesBDP = new List<String>();
		private List<String> serviceCategories = new List<String>();
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Login);

			
			editLogin = (EditText)FindViewById (Resource.Id.editLogin);
			editPassword = (EditText)FindViewById (Resource.Id.editPassword);
			btnSignIn = (Button)FindViewById (Resource.Id.btnSignIn);
			btnSignIn.Click += async (sender, e) => {

				Drawable errorIcon = Resources.GetDrawable(Resource.Drawable.error); 
				errorIcon.SetBounds(0,0,errorIcon.IntrinsicWidth,errorIcon.IntrinsicHeight);
				bool trouve = false;
				bool empty = false;
				if (editLogin.Text.Length <= 0 || editPassword.Text.Length <= 0) {
					editLogin.SetError ("Login required!", errorIcon);
					if (editPassword.Text.Length <= 0) {
						editPassword.SetError ("Password required!", errorIcon);
					}
					empty = true;
				} else {
					/*int nbAccounts = accounts.Count ();
				string testLogin;
				string testPassword;

				int i = 0;
				while (i< nbAccounts && ! trouve) {
					testLogin = loginPrefs.GetString ("login_" + i, null);
					testPassword = loginPrefs.GetString ("password_" + i, null);
					if (editLogin.Text.ToString () == testLogin && editPassword.Text.ToString () == testPassword) {
						loginIntent = new Intent (this, typeof(MainActivity));
						loginIntent.PutExtra ("id",editPassword.Text.ToString () );
						StartActivity (loginIntent);
						trouve = true;
					} else {
						i++;
					}

 
				}*/

					string url = "http://192.168.1.12:8383/IBeaconService/rest/ClientServiceWS/authenticateUser?login="+
						editLogin.Text+"&password="+editPassword.Text;

					// Fetch the weather information asynchronously, 
					// parse the results, then update the screen:
					JsonValue json = await FetchUserAsync(url);

					ParseAndProcess(json);

				}
			};
			// Create your application here
		}

	

		void HandleLoginClick (object sender, EventArgs ea) {
			/*login = loginPrefs.GetString ("login_0", null);
			password = loginPrefs.GetString ("password_0", null);
			login1 = loginPrefs.GetString ("login_1", null);
			password1 = loginPrefs.GetString ("password_1", null);*/

			 
		}
		// Authenticates the user 
		private async Task<JsonValue> FetchUserAsync (string url)
		{
			// Create an HTTP web request using the URL:
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
			request.ContentType = "application/json";
			request.Method = "GET";

			// Send the request to the server and wait for the response:
 			using (WebResponse response = await request.GetResponseAsync ())
			{
				// Get a stream representation of the HTTP web response:
				using (Stream stream = response.GetResponseStream ())
				{
					// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run (() => JsonObject.Load (stream));
					Console.Out.WriteLine("Response: {0}", jsonDoc.ToString ());

					// Return the JSON document:
					return jsonDoc;
				}
			}
		}
		public void saveArray()
		{
			
			loginEditor.PutInt("login_size", accounts.Count()); /* sKey is an array */ 
			loginEditor.PutInt("service_size", services.Count()); /* sKey is an array */ 
			loginEditor.PutInt("category_size", serviceCategories.Count());
			loginEditor.PutInt("devicesBdp_size", devicesBDP.Count());
			string[] valeurs;
			for(int i=0;i<accounts.Count();i++)  
			{
				valeurs = accounts [i].ToString ().Split ('|');
				loginEditor.Remove("login_" + i);
				loginEditor.PutString("login_" + i,valeurs[0]);
				loginEditor.Remove("password_" + i);
				loginEditor.PutString("password_" + i, valeurs[1]);  
			}
			for(int i=0;i<services.Count();i++)  
			{
				valeurs = services [i].ToString ().Split ('|');
				loginEditor.Remove("serviceId_" + i);
				loginEditor.PutString("serviceId_" + i,valeurs[0]);
				loginEditor.Remove("contenuService_" + i);
				loginEditor.PutString("contenuService_" + i, valeurs[1]);  
				loginEditor.Remove("locationService_" + i);
				loginEditor.PutString("locationService_" + i,valeurs[2]);  

			}

			for(int i=0;i<devicesBDP.Count();i++)  
			{
				valeurs = devicesBDP [i].ToString ().Split ('|');
				loginEditor.Remove("BDPUuId_" + i);
				loginEditor.PutString("BDPUuId_" + i,valeurs[0]);
				loginEditor.Remove("BDPLocation_" + i);
				loginEditor.PutString("BDPLocation_" + i, valeurs[1]);  

			}

			for(int i=0;i<serviceCategories.Count();i++)  
			{
				valeurs = serviceCategories [i].ToString ().Split ('|');
				loginEditor.Remove("ServiceCategoriesId_" + i);
				loginEditor.PutString("ServiceCategoriesId_" + i,valeurs[0]);
				loginEditor.Remove("ServiceCategoriesName_" + i);
				loginEditor.PutString("ServiceCategoriesName_" + i, valeurs[1]);  

			}
			loginEditor.Commit();     
		}



		// Parse the user data, then permits , user accessing the app
		private void ParseAndProcess (JsonValue json)
		{
			// Extract the array of name/value results for the field name "weatherObservation". 
			JsonValue authResults = json;

			if (json["idUser"] == null) {
				AlertDialog.Builder alert = new AlertDialog.Builder (this);
				/*editLogin.SetError ("",errorIcon);
				editPassword.SetError ("",errorIcon);*/
				alert.SetTitle ("Login Error");
				alert.SetMessage ("Mismatched Login and Password");
				alert.SetPositiveButton ("Retry", (senderAlert, args) => {
					//change value write your own set of instructions
					//you can also create an event for the same in xamarin
					//instead of writing things here
				} );
				alert.Show ();

			} else {
				var userInfo = new UserInfo ();
				String stringValue = authResults.ToString();
				userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo> (stringValue);



				loginPrefs = GetSharedPreferences("loginPrefs",FileCreationMode.Private);
				loginEditor = loginPrefs.Edit ();

				accounts.Add (userInfo.userEmail+"|"+editPassword.Text);		
				foreach (PrivateService pService in userInfo.privateServices) {
					services.Add (userInfo.idUser+"|"+pService.serviceName+" "+pService.serviceDescription+"|" +pService.locations[0].idLocation);
				}
				foreach (Bdp bdp in userInfo.bdps) {
					devicesBDP.Add (bdp.uuId + "|" + bdp.location.idLocation);
				}
				foreach (ServiceCategory serviceCategory in userInfo.serviceCategories) {
					serviceCategories.Add (serviceCategory.idServiceCategory + "|" + serviceCategory.categoryName);
				}
				saveArray ();


				loginIntent = new Intent (this, typeof(MainActivity));
				loginIntent.PutExtra ("id",editPassword.Text.ToString () );
				StartActivity (loginIntent);
			}

			// Extract the "stationName" (location string) and write it to the location TextBox:
			//location.Text = weatherResults["stationName"];

		}
	}
}

