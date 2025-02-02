﻿using System;
using System.Runtime.InteropServices;
namespace SDRSharp.Rtl_433
{
     internal static class NativeMethods //NativeMethods safeNativeMethods ...
    {
        [DllImport("kernel32.dll")] [return: MarshalAs(UnmanagedType.Bool)] static extern internal Boolean FreeConsole(); 
        
        [DllImport("kernel32.dll")] [return: MarshalAs(UnmanagedType.Bool)] static extern internal Boolean AllocConsole();
        private const String LibRtl_433 = "rtl_433";

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void receiveMessagesCallback([In]char[] text, [In]Int32 len);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void ptrFct([In, MarshalAs(UnmanagedType.LPStr)] String message);

        [DllImport("rtl_433", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "test_dll_get_version")]
        internal static extern IntPtr IntPtr_Pa_GetVersionText();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void ptrFctInit(IntPtr _ptrCbData,
            Int32 _bufNumber,
            UInt32 _bufLength,
            IntPtr _ptrCtx,
            IntPtr _ptrCfg);

        [DllImport("rtl_433", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern void rtl_433_call_main([Out] ptrFct fctMessage, [Out] ptrFctInit fctInitCbData, [Out] UInt32 param_samp_rate, [Out] Int32 param_sample_size, [Out] UInt32 disabled, [Out] Int32 argc, String[] args);  // 

        [DllImport("rtl_433", CallingConvention = CallingConvention.StdCall)]
        internal static extern void receive_buffer_cb([Out] byte[] iq_buf, [Out] UInt32 len, [Out]  IntPtr ctx);

        [DllImport("rtl_433", CallingConvention = CallingConvention.StdCall)]
        internal static extern void stop_sdr([Out] IntPtr ctx);

        [DllImport("rtl_433", CallingConvention = CallingConvention.StdCall)]
        internal static extern void free_console();

        [DllImport("rtl_433", CallingConvention = CallingConvention.StdCall)]
        internal static extern void setFrequency([Out] UInt32 frequency);

        [DllImport("rtl_433", CallingConvention = CallingConvention.StdCall)]
        internal static extern void setCenterFrequency([Out] UInt32 centerFrequency);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        internal struct list
        {
            internal IntPtr elems;   // void** elems;
            internal UInt32 size;        //size_t
            internal UInt32 len;        //size_t
        }
        //list_t;
        internal enum conversion_mode_t : Int32
        {
            CONVERT_NATIVE,
            CONVERT_SI,
            CONVERT_CUSTOMARY
        }
        internal enum time_mode_t : Int32
        {
            REPORT_TIME_DEFAULT,
            REPORT_TIME_DATE,
            REPORT_TIME_SAMPLES,
            REPORT_TIME_UNIX,
            REPORT_TIME_ISO,
            REPORT_TIME_OFF
        }


        /// state data for pulse_FSK_detect()
        internal enum _fsk_state : Int32
        {
            PD_FSK_STATE_INIT = 0,   //< Initial frequency estimation
            PD_FSK_STATE_FH = 1,     //< High frequency (pulse)
            PD_FSK_STATE_FL = 2,     //< Low frequency (gap)
            PD_FSK_STATE_ERROR = 3   //< Error - stay here until cleared
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        internal struct pulse_FSK_state_t
        {
            internal UInt32 fm_f1_est; ///< Estimate for the F1 frequency for FSK
            internal Int32 fm_f2_est; ///< Estimate for the F2 frequency for FSK
            internal UInt32 fsk_pulse_length; ///< Counter for internal FSK pulse detection
            internal _fsk_state fsk_state;
            internal Int16 var_test_max;
            internal Int16 var_test_min;
            internal Int16 maxx;
            internal Int16 minn;
            internal Int16 midd;
            internal Int32 skip_samples;
        }


        internal enum _ook_state : Int32
        {
            PD_OOK_STATE_IDLE = 0,
            PD_OOK_STATE_PULSE = 1,
            PD_OOK_STATE_GAP_START = 2,
            PD_OOK_STATE_GAP = 3
        }

        /// Internal state data for pulse_pulse_package()
        struct pulse_detect
        {
            internal Int32 use_mag_est;          ///< Whether the envelope data is an amplitude or magnitude.
            internal Int32 ook_fixed_high_level; ///< Manual detection level override, 0 = auto.
            internal Int32 ook_min_high_level;   ///< Minimum estimate of high level (-12 dB: 1000 amp, 4000 mag).
            internal Int32 ook_high_low_ratio;   ///< Default ratio between high and low (noise) level (9 dB: x8 amp, 11 dB: x3.6 mag).

            internal _ook_state ook_state;
            internal Int32 pulse_length; ///< Counter for internal pulse detection
            internal Int32 max_pulse;    ///< Size of biggest pulse detected

            internal Int32 data_counter;    ///< Counter for how much of data chunk is processed
            internal Int32 lead_in_counter; ///< Counter for allowing initial noise estimate to settle

            internal Int32 ook_low_estimate;  ///< Estimate for the OOK low level (base noise level) in the envelope data
            internal Int32 ook_high_estimate; ///< Estimate for the OOK high level

            internal Int32 verbosity; ///< Debug output verbosity, 0=None, 1=Levels, 2=Histograms

            internal pulse_FSK_state_t FSK_state;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        internal struct sdr_dev
        {
            internal UInt32 rtl_tcp;  //SOCKET
            internal UInt32 rtl_tcp_freq; ///< last known center frequency, rtl_tcp only.
            internal UInt32 rtl_tcp_rate; ///< last known sample rate, rtl_tcp only.

            //# ifdef SOAPYSDR
            //            SoapySDRDevice* soapy_dev;
            //            SoapySDRStream* soapy_stream;
            //            double fullScale;
            //#endif

            //# ifdef RTLSDR
            internal IntPtr rtlsdr_dev;  //rtlsdr_dev_t*
            internal IntPtr rtlsdr_cb;   //sdr_event_cb_t
            internal IntPtr rtlsdr_cb_ctx;    //void*
                                            //#endif

            internal String dev_info;

            internal Int32 running;
            internal Int32 polling;
            internal IntPtr buffer;   //void*
            internal UInt32 buffer_size;   //size_t

            internal Int32 sample_size;
            Int32 sample_signed;
            internal Int32 apply_rate;
            internal Int32 apply_freq;
            internal Int32 apply_corr;
            internal Int32 apply_gain;
            internal UInt32 sample_rate;
            internal Int32 freq_correction;
            internal UInt32 center_frequency;
            internal String gain_str;
        }
        /// Data for a compact representation of generic pulse train.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        internal struct pulse_data
        {
            internal UInt64 offset;            ///< Offset to first pulse in number of samples from start of stream.
            internal UInt32 sample_rate;       ///< Sample rate the pulses are recorded with.
            internal UInt32 depth_bits;        ///< Sample depth in bits.
            internal UInt32 start_ago;         ///< Start of first pulse in number of samples ago.
            internal UInt32 end_ago;           ///< End of last pulse in number of samples ago.
            internal UInt32 num_pulses;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1200)]
            internal Int32[] pulse;   ///< Width of pulses (high) in number of samples.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1200)]
            internal Int32[] gap;     ///< Width of gaps between pulses (low) in number of samples.
            internal Int32 ook_low_estimate;       ///< Estimate for the OOK low level (base noise level) at beginning of package.
            internal Int32 ook_high_estimate;      ///< Estimate for the OOK high level at end of package.
            internal Int32 fsk_f1_est;             ///< Estimate for the F1 frequency for FSK.
            internal Int32 fsk_f2_est;             ///< Estimate for the F2 frequency for FSK.
            internal Single freq1_hz;
            internal Single freq2_hz;
            internal Single centerfreq_hz;
            internal Single range_db;
            internal Single rssi_db;
            internal Single snr_db;
            internal Single noise_db;
        }
        //pulse_data_t;

        internal const Int32 FILTER_ORDER = 1;

        /// Filter state buffer.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        internal struct filter_state
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FILTER_ORDER)]
            internal Int16[] y;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = FILTER_ORDER)]
            internal Int16[] x;
        }
        // filter_state_t;

        /// FM_Demod state buffer.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        internal struct demodfm_state
        {
            internal Int32 xr;        ///< Last I/Q sample, real part
            internal Int32 xi;        ///< Last I/Q sample, imag part
            internal Int32 xf;        ///< Last Instantaneous frequency
            internal Int32 yf;        ///< Last Instantaneous frequency, low pass filtered
            internal UInt32 rate;     ///< Current sample rate
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            internal Int32[] alp_16; ///< Current low pass filter A coeffs, 16 bit
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            internal Int32[] blp_16; ///< Current low pass filter B coeffs, 16 bit
            internal Int32 bidon;   //-------------------------------------------------------<why?
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            internal Int64[] alp_32; ///< Current low pass filter A coeffs, 32 bit
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            internal Int64[] blp_32; ///< Current low pass filter B coeffs, 32 bit
        }
        //demodfm_state_t;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        internal struct file_info
        {
            internal UInt32 format;
            internal UInt32 raw_format;
            internal UInt32 center_frequency;
            internal UInt32 sample_rate;
            internal String spec;
            internal String path;
            internal IntPtr file;        //FILE*
        }
        //file_info_t;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        internal struct timeval
        {
            internal Int32 tv_sec;         /* seconds */
            internal Int32 tv_usec;        /* and microseconds */
        }
        internal const Int32 MAXIMAL_BUF_LENGTH = (256 * 16384);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        internal struct dm_state
        {
            float auto_level;
            float squelch_offset;
            internal Single level_limit;
            float noise_level;
            float min_level_auto;
            internal Single min_level;
            internal Single min_snr;
            internal Single low_pass;
            internal Int32 use_mag_est;
            internal Int32 detect_verbosity;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMAL_BUF_LENGTH)]
            internal Int16[] am_buf;  // AM demodulated signal (for OOK decoding)
            //union {
            //    // These buffers aren't used at the same time, so let's use a union to save some memory
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMAL_BUF_LENGTH)]
            internal Int16[] fm;  // FM demodulated signal (for FSK decoding)
                                //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMAL_BUF_LENGTH)]
                                //    internal UInt16[] temp;  // Temporary buffer (to be optimized out..)
                                //}
                                //buf;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMAL_BUF_LENGTH)]
            internal Byte[] u8_buf; // format conversion buffer
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMAL_BUF_LENGTH)]
            internal Single[] f32_buf; // format conversion buffer
            internal Int32 sample_size; // CU8: 1, CS16: 2--->*2 in rtl_433
            internal IntPtr pulse_detect;        //pulse_detect_t*
            internal filter_state lowpass_filter_state;
            internal Int32 bidon;         //-----------------------------<why?
            internal demodfm_state demod_FM_state;     //demodfm_state
            internal Int32 enable_FM_demod;
            internal UInt32 fsk_pulse_detect_mode;
            internal UInt32 frequency;
            internal IntPtr samp_grab;        //samp_grab_t*
            internal IntPtr am_analyze;          //am_analyze_t* 
            internal Int32 analyze_pulses;
            internal file_info load_info;
            internal list dumper;

            /* Protocol states */
            internal list r_devs;
            internal Int32 bidon1;         //-----------------------------<why?
            internal pulse_data pulse_data;       //pulse_data
            internal pulse_data fsk_pulse_data;   //pulse_data
            internal UInt32 frame_event_count;
            internal UInt32 frame_start_ago;
            internal UInt32 frame_end_ago;
            internal timeval now;        //struct  int32 and not Int64 -----------------------------<why?
            internal Single sample_file_pos;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Pack = 4)]
        internal struct r_cfg
        {
            [FieldOffset(0)]    //,MarshalAs(UnmanagedType.LPStr)
            internal String dev_query;
            [FieldOffset(4)]
            internal String dev_info;
            [FieldOffset(4 + 4)]
            internal String gain_str;
            [FieldOffset(8 + 4)]
            internal String settings_str;
            [FieldOffset(12 + 4)]
            internal Int32 ppm_error;
            [FieldOffset(16 + 4)]
            internal UInt32 out_block_size;
            [FieldOffset(20 + 4)]
            internal String test_data;
            [FieldOffset(24 + 4)]
            internal list in_files;
            [FieldOffset(28 + 12)]
            internal String in_filename;
            [FieldOffset(40 + 4)]
            internal Int32 replay;
            [FieldOffset(44 + 4)]
            internal Int32 hop_now;
            [FieldOffset(48 + 4)]
            internal Int32 exit_async;
            [FieldOffset(52 + 4)]
            internal Int32 exit_code; ///< 0=no err, 1=params or cmd line err, 2=sdr device read error, 3=usb init error, 5=USB error (reset), other=other error
            [FieldOffset(56 + 4)]
            internal Int32 frequencies;
            [FieldOffset(60 + 4)]
            internal Int32 frequency_index;
            [FieldOffset(64 + 4)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            internal UInt32[] frequency;
            [FieldOffset(68 + 32 * 4)]
            internal UInt32 center_frequency;
            [FieldOffset(196 + 4)]
            internal Int32 fsk_pulse_detect_mode;
            [FieldOffset(200 + 4)]
            internal Int32 hop_times;
            [FieldOffset(204 + 4)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            internal Int32[] hop_time;
            [FieldOffset(208 + 32 * 4 + 4)]    //-----------------------------------<why +4?
            internal Int64 hop_start_time;   //time_t
            [FieldOffset(340 + 8)]
            internal Int32 duration;
            [FieldOffset(348 + 4)]
            internal Int64 stop_time;   //time_t
            [FieldOffset(352 + 8)]
            internal Int32 after_successful_events_flag;
            [FieldOffset(360 + 4)]
            internal UInt32 samp_rate;
            [FieldOffset(364 + 4)]
            internal UInt64 input_pos;
            [FieldOffset(368 + 8)]
            internal UInt32 bytes_to_read;
            [FieldOffset(376 + 4)]
            internal IntPtr dev;   //  struct sdr_dev *dev;
            [FieldOffset(380 + 4)]
            internal Int32 grab_mode; ///< Signal grabber mode: 0=off, 1=all, 2=unknown, 3=known
            [FieldOffset(384 + 4)]
            internal Int32 raw_mode; ///< Raw pulses printing mode: 0=off, 1=all, 2=unknown, 3=known
            [FieldOffset(388 + 4)]
            internal Int32 verbosity; ///< 0=normal, 1=verbose, 2=verbose decoders, 3=debug decoders, 4=trace decoding.
            [FieldOffset(392 + 4)]
            internal Int32 verbose_bits;
            [FieldOffset(396 + 4)]
            internal conversion_mode_t conversion_mode;
            [FieldOffset(400 + 4)]
            internal Int32 report_meta;
            [FieldOffset(404 + 4)]
            internal Int32 report_noise;
            [FieldOffset(408 + 4)]
            internal Int32 report_protocol;
            [FieldOffset(412 + 4)]
            internal time_mode_t report_time;
            [FieldOffset(416 + 4)]
            internal Int32 report_time_hires;
            [FieldOffset(420 + 4)]
            internal Int32 report_time_tz;
            [FieldOffset(424 + 4)]
            internal Int32 report_time_utc;  //commencer la
            [FieldOffset(428 + 4)]
            internal Int32 report_description;
            [FieldOffset(432 + 4)]
            internal Int32 report_stats;
            [FieldOffset(436 + 4)]
            internal Int32 stats_interval;
            [FieldOffset(440 + 4)]
            internal Int32 stats_now;
            [FieldOffset(444 + 4)]       //-----------------------------------<why +4?
            internal Int64 stats_time;     //time_t
            [FieldOffset(448 + 8)]
            internal Int32 no_default_devices;
            [FieldOffset(456 + 4)]
            internal IntPtr devices;       //internal struct r_device *devices;
            [FieldOffset(460 + 4)]
            internal UInt16 num_r_devices;
            [FieldOffset(464 + 4)]        //not 2 ?
            internal list data_tags;
            [FieldOffset(468 + 12)]
            internal list output_handler;
            [FieldOffset(480 + 12)]
            internal IntPtr demod;     //internal struct dm_state *demod;
            [FieldOffset(492 + 4)]
            internal String sr_filename;
            [FieldOffset(496 + 4)]
            internal Int32 sr_execopen;
            [FieldOffset(500 + 4)]
            //internal Int32 old_model_keys;
            //[FieldOffset(508+4)]    //-----------------------------------<why?
            /* stats*/
            internal Int64 frames_since;    //time_t
            [FieldOffset(504 + 8)]
            internal UInt32 frames_count; ///< stats counter for interval
            [FieldOffset(512 + 4)]
            internal UInt32 frames_fsk; ///< stats counter for interval
            [FieldOffset(516 + 4)]
            internal UInt32 frames_events; ///< stats counter for interval
            [FieldOffset(520 + 4)]
            internal IntPtr mgr;    //internal struct mg_mgr *mgr;
        }
    }
}