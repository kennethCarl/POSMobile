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
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Support.V7.App;
using Android.Support.V4.Widget;

namespace POSV1
{
    class AppActionBarDrawerToggle:  SupportActionBarDrawerToggle
    {
        private ActionBarActivity mHost;
        private DrawerLayout mDrawerLayout;
        private int mOpenDrawerDesc;
        private int mCloseDrawerDesc;
        public AppActionBarDrawerToggle(ActionBarActivity host, DrawerLayout drawerLayout, int openDrawerDesc, int closeDrawerDesc) 
        :base(host, drawerLayout, openDrawerDesc, closeDrawerDesc)
        {
            mHost = host;
            mOpenDrawerDesc = openDrawerDesc;
            mCloseDrawerDesc = closeDrawerDesc;
        }
        public override void OnDrawerOpened(Android.Views.View drawerView)
        {
            int drawerType = (int)drawerView.Tag;

            if (drawerType == 0)
            {
                base.OnDrawerOpened(drawerView);
                mHost.SupportActionBar.SetTitle(mOpenDrawerDesc);
            }
        }
        public override void OnDrawerClosed(Android.Views.View drawerView)
        {
            int drawerType = (int)drawerView.Tag;

            if (drawerType == 0)
            {
                base.OnDrawerClosed(drawerView);
                mHost.SupportActionBar.SetTitle(mCloseDrawerDesc);
            }
        }
        public override void OnDrawerSlide(Android.Views.View drawerView, float slideOffset)
        {
            int drawerType = (int)drawerView.Tag;

            if (drawerType == 0)
            {
                base.OnDrawerSlide(drawerView, slideOffset);
            }
        }

        public void setCloseDrawerDesc(int id){
            mCloseDrawerDesc = id;
        }
    }
}