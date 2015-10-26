
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
using Android.Support.V4.App;
using Android.Support.V4.View;
namespace AndroidBeacon
{
	public class ServiceBeaconAdapter : FragmentStatePagerAdapter{
		private List<Android.Support.V4.App.Fragment> _fragmentList = new List<Android.Support.V4.App.Fragment>();
		public ServiceBeaconAdapter(Android.Support.V4.App.FragmentManager fm) : base(fm){
		}

		public override int Count {
			get {
				return _fragmentList.Count;
			}
		}

		public void addFragment(ServiceBeacon serviceBeconFragment){
			_fragmentList.Add (serviceBeconFragment);
		}
		public void addFragmentView(Func<LayoutInflater, ViewGroup, Bundle, View> view){
			_fragmentList.Add (new ServiceBeacon(view));
		}
		public override Android.Support.V4.App.Fragment GetItem (int position)
		{
			return _fragmentList [position];
		}

		
	}
		


	public class ServiceBeacon : Android.Support.V4.App.Fragment
	{
		private Func<LayoutInflater, ViewGroup, Bundle, View> _view;
		/*public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.Page, container, false);
			return view;
		}*/
		public ServiceBeacon(Func<LayoutInflater, ViewGroup, Bundle, View> view)
		{	_view = view;
		}


		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			return _view(inflater, container, savedInstanceState);
		}
	}


	public class FakeContent : Java.Lang.Object, TabHost.ITabContentFactory 
		{
			private Context _context;

			public FakeContent(Context context) {
				_context = context;
			}

			public View CreateTabContent (string tag)
			{
				var v = new View(_context);
				v.SetMinimumHeight(0);
				v.SetMinimumWidth(0);
				return v;
			}


	}





}

