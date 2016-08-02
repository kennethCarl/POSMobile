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
using System.Reflection;

namespace POSMobile.CustomClass
{
    class DatePickerFragment :  DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();
        public DateTime currentDateTime;
        public bool isDateSet = false;
        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> _dateSelectedHandler = delegate { };
        Action<DateTime> _dateCancelHandler = delegate { };
        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected, Action<DateTime> onDateCancel)
        {
            DatePickerFragment frag = new DatePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            frag._dateCancelHandler = onDateCancel;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                           this,
                                                           currentDateTime.Year,
                                                           currentDateTime.Month - 1,
                                                           currentDateTime.Day);
            this.isDateSet = false;
            return dialog;
        }
        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            this.isDateSet = true;
            _dateSelectedHandler(selectedDate);
        }
        public override void OnCancel(IDialogInterface dialog)
        {
            DateTime selectedDate = new DateTime(currentDateTime.Year,
                                                           currentDateTime.Month - 1,
                                                           currentDateTime.Day);
            _dateCancelHandler(selectedDate);
        }
        public override void OnDismiss(IDialogInterface dialog)
        {
            if (!this.isDateSet)
                this.OnCancel(dialog);
 	         base.OnDismiss(dialog);
        }
    }
}