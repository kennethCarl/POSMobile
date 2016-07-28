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

namespace POSV1.CustomeClasses
{
    class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
    {
        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(TimePickerFragment).Name.ToUpper();
        public DateTime currentDateTime;
        public string Time;
        public string Minutes;
        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> _timeSelectedHandler = delegate { };

        public static TimePickerFragment NewInstance(Action<DateTime> onTimeSelected)
        {
            TimePickerFragment frag = new TimePickerFragment();
            frag._timeSelectedHandler = onTimeSelected;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            //DateTime currently = DateTime.Now;
            TimePickerDialog dialog = new TimePickerDialog(Activity,
                                                           this,
                                                           currentDateTime.Hour,
                                                           currentDateTime.Minute,
                                                           false);

            dialog.SetCancelable(false);
            return dialog;
        }

        public void OnTimeSet(TimePicker view, int hour, int minutes)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, hour, minutes, 0);
            _timeSelectedHandler(selectedDate);
        }
    }
}