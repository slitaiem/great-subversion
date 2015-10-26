using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RadiusNetworks.IBeaconAndroid;
using Android.Support.V4.View;
using Android.Support.V4.App;

using System.Collections;


using System.Collections.Generic;

using System.Text;

using Java.Lang;

namespace AndroidBeacon
{
	[Activity (Label = "Android Beacon")]
	public class MainActivity : Android.Support.V4.App.FragmentActivity  , TabHost.IOnTabChangeListener, ViewPager.IOnPageChangeListener, IBeaconConsumer
	{


		private ViewPager viewPager;
		private TabHost tabHost;
		const string UUID = "B9407F30F5F8466EAFF925556B57FE6D";
		const string BEACON_ID = "iOSBeacon";
		IBeaconManager beaconMgr;
		MonitorNotifier monitorNotifier;
		RangeNotifier rangeNotifier;
		Region monitoringRegion;
		//Region rangingRegion;
		TextView beaconStatusLabel;
		IEnumerable myProcessedBeacons;
		private ISharedPreferences mainPrefs;
		private ISharedPreferencesEditor mainEditor;
		string userID;
		int serviceNumer = 0;
		int categoryNumber = 0;
		Button btnSignOut;

		public MainActivity ()
		{
			
			beaconMgr = IBeaconManager.GetInstanceForApplication (this);

			monitorNotifier = new MonitorNotifier ();
			monitoringRegion = new Region (BEACON_ID, null, null, null);

			rangeNotifier = new RangeNotifier ();

			//rangingRegion = new Region (BEACON_ID, null, null, null);
		}


		public void OnTabChanged (string tabId)
		{
			int tb = tabHost.CurrentTab;
			viewPager.SetCurrentItem(tb,false);
			setSelectedTabColor ();
		}

		public void OnPageScrollStateChanged (int state)
		{
			
		}


		public void OnPageScrolled (int position, float positionOffset, int positionOffsetPixels)
		{
			
		}


		public void OnPageSelected (int position)
		{
			tabHost.CurrentTab = position;
		}

		public void OnIBeaconServiceConnect ()
		{
			beaconMgr.SetMonitorNotifier (monitorNotifier);
			/*beaconMgr.SetRangeNotifier (rangeNotifier);

			beaconMgr.StartMonitoringBeaconsInRegion (monitoringRegion);
			beaconMgr.StartRangingBeaconsInRegion (rangingRegion);*/
			beaconMgr.SetRangeNotifier(rangeNotifier);
			beaconMgr.StartRangingBeaconsInRegion (monitoringRegion);

			beaconMgr.StartMonitoringBeaconsInRegion(monitoringRegion);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);



			userID = Intent.GetStringExtra ("id") ?? "-1";
			mainPrefs = GetSharedPreferences("loginPrefs",FileCreationMode.Private);
			mainEditor = mainPrefs.Edit ();
			serviceNumer = mainPrefs.GetInt ("service_size", -1);
			categoryNumber = mainPrefs.GetInt ("category_size", 0);
			SetContentView (Resource.Layout.Main);

			tabHost = FindViewById<TabHost> (Android.Resource.Id.TabHost);

			tabHost.Setup ();



			for (int i = 0; i < categoryNumber; i++) {

				TabHost.TabSpec tabSpec;
				tabSpec = tabHost.NewTabSpec(mainPrefs.GetString ("ServiceCategoriesName_" + i, null).ToLower());
				tabSpec.SetIndicator(mainPrefs.GetString ("ServiceCategoriesName_" + i, null).ToLower());
				tabSpec.SetContent(new FakeContent(this));
				tabHost.AddTab(tabSpec);
			}
			tabHost.SetOnTabChangedListener(this);
			setSelectedTabColor ();
			/*for(int i = 0; i < tabHost.TabWidget.ChildCount; i++) {
				View v = tabHost.TabWidget.GetChildTabViewAt(i);

				// Look for the title view to ensure this is an indicator and not a divider.

				v.SetBackgroundResource(Resource.Drawable.apptheme_tab_indicator_holo);
			}*/
			viewPager = FindViewById<ViewPager> (Resource.Id.view);
			var adaptor = new ServiceBeaconAdapter (SupportFragmentManager);

			for (int i = 0; i < categoryNumber; i++) {
				adaptor.addFragmentView ((k, v, b) => {
					var view = k.Inflate (Resource.Layout.Page, v, false);
					var myText = view.FindViewById<TextView> (Resource.Id.textView1);
					myText.Text = mainPrefs.GetString ("ServiceCategoriesName_" + i, null).ToLower();
					return view;
				});
			}


			viewPager.Adapter = adaptor;//new ServiceBeaconAdapter (SupportFragmentManager);
			viewPager.SetOnPageChangeListener(this);

			beaconStatusLabel = FindViewById<TextView> (Resource.Id.beaconStatusLabel);

			beaconMgr.Bind (this);

			//myProcessedBeacons = new JavaDictionary<string,string>();
			monitorNotifier.EnterRegionComplete += EnteredRegion;
			monitorNotifier.ExitRegionComplete += ExitedRegion;

			rangeNotifier.DidRangeBeaconsInRegionComplete += HandleBeaconsInRegion;
		}

		private void setSelectedTabColor() {
			TabWidget widget = tabHost.TabWidget;
			for(int i = 0; i < widget.ChildCount; i++) {
				View v = widget.GetChildTabViewAt(i);
				if (i == tabHost.CurrentTab) {
					v.SetBackgroundColor(Android.Graphics.Color.Argb(222, 111, 196, 58));
				} else {
					v.SetBackgroundColor(Android.Graphics.Color.Argb(222, 136, 205, 91));
				}

			}
		}
		void EnteredRegion (object sender, MonitorEventArgs e)
		{
			ShowMessage ("Welcome!");
		}

		void ExitedRegion (object sender, MonitorEventArgs e)
		{
			ShowMessage ("See you soon!");
		}

		void HandleBeaconsInRegion (object sender, RangeEventArgs e)
		{
			if (e.Beacons.Count > 0) {
				foreach (var beacon in e.Beacons) {
					/*if (!((JavaDictionary)myProcessedBeacons).Contains(beacon.ProximityUuid)) {
						((JavaDictionary)myProcessedBeacons).Add (beacon.ProximityUuid, 
							beacon.Major.ToString () + "|" + beacon.Minor.ToString ());*/
					for (int i = 0; i < serviceNumer; i++) {
						string testUUID = mainPrefs.GetString ("BeaconService_" + i, null).ToLower();
						string testUserID = mainPrefs.GetString ("serviceId_" + i, null);
						if (testUUID == beacon.ProximityUuid.Replace("-","") &&	testUserID == userID) {
							switch ((ProximityType)beacon.Proximity) {
							case ProximityType.Immediate:
								ShowMessage ("ProximityType : Immediate, " +  mainPrefs.GetString ("contenuService_" + i, null) , false);
								break;
							case ProximityType.Near:
								ShowMessage ("ProximityType : Near, " +  mainPrefs.GetString ("contenuService_" + i, null) , false);
								break;
							case ProximityType.Far:
									ShowMessage ("ProximityType : Far, " +  mainPrefs.GetString ("contenuService_" + i, null) , false);
								break;
							case ProximityType.Unknown:
								ShowMessage ("Beacon proximity unknown");
								break;
							}
						}
					}

						
					//}

				}
				/*var beacon = e.Beacons.FirstOrDefault ();*/


			}
		}

		void ShowMessage(string message)
		{
			ShowMessage (message, false);
		}

		void ShowMessage (string message, bool showCoupon)
		{
			RunOnUiThread (() => {
				beaconStatusLabel.Text = message;

				var couponView = FindViewById<ImageView> (Resource.Id.imageView);

				if (showCoupon)
					couponView.SetImageResource (Resource.Drawable.qrcode);
				else
					couponView.SetImageResource (0);
			});
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();

			monitorNotifier.EnterRegionComplete -= EnteredRegion;
			monitorNotifier.ExitRegionComplete -= ExitedRegion;

			rangeNotifier.DidRangeBeaconsInRegionComplete -= HandleBeaconsInRegion;

			beaconMgr.StopMonitoringBeaconsInRegion (monitoringRegion);
			beaconMgr.StopRangingBeaconsInRegion (monitoringRegion);
			beaconMgr.UnBind (this);
		}
		/*public override void OnBackPressed()
		{
			logOut (null, null);
		}*/

		void logOut (object sender, EventArgs ea){
			//StartActivity (typeof(LoginActivity));
		}
	}
}