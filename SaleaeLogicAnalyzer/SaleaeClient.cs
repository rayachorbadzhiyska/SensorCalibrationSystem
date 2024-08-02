﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace SensorCalibrationSystem.SaleaeLogicAnalyzer
{
    public class SaleaeClient
    {
        public static bool PrintCommandsToConsole = false;

        TcpClient Socket;
        NetworkStream Stream;
        int port;
        string host;

        //Command strings
        const string set_trigger_cmd = "SET_TRIGGER";
        const string get_num_samples_cmd = "GET_NUM_SAMPLES";
        const string set_num_samples_cmd = "SET_NUM_SAMPLES";
        const string get_sample_rate_cmd = "GET_SAMPLE_RATE";
        const string set_sample_rate_cmd = "SET_SAMPLE_RATE";
        const string set_capture_seconds_cmd = "SET_CAPTURE_SECONDS";
        const string capture_to_file_cmd = "CAPTURE_TO_FILE";
        const string save_to_file_cmd = "SAVE_TO_FILE";
        const string load_from_file_cmd = "LOAD_FROM_FILE";
        const string export_data_cmd = "EXPORT_DATA";
        const string export_data2_cmd = "EXPORT_DATA2";
        const string get_all_sample_rates_cmd = "GET_ALL_SAMPLE_RATES";
        const string get_analyzers_cmd = "GET_ANALYZERS";
        const string export_analyzer_cmd = "EXPORT_ANALYZER";
        const string get_inputs_cmd = "GET_INPUTS";
        const string capture_cmd = "CAPTURE";
        const string stop_capture_cmd = "STOP_CAPTURE";
        const string get_capture_pretrigger_buffer_size_cmd = "GET_CAPTURE_PRETRIGGER_BUFFER_SIZE";
        const string set_capture_pretrigger_buffer_size_cmd = "SET_CAPTURE_PRETRIGGER_BUFFER_SIZE";
        const string get_connected_devices_cmd = "GET_CONNECTED_DEVICES";
        const string select_active_device_cmd = "SELECT_ACTIVE_DEVICE";
        const string get_active_channels_cmd = "GET_ACTIVE_CHANNELS";
        const string set_active_channels_cmd = "SET_ACTIVE_CHANNELS";
        const string reset_active_channels_cmd = "RESET_ACTIVE_CHANNELS";
        const string set_performance_cmd = "SET_PERFORMANCE";
        const string get_performance_cmd = "GET_PERFORMANCE";
        const string is_processing_complete_cmd = "IS_PROCESSING_COMPLETE";
        const string is_analyzer_complete_cmd = "IS_ANALYZER_COMPLETE";
        const string set_digital_voltage_option_cmd = "SET_DIGITAL_VOLTAGE_OPTION";
        const string get_digital_voltage_options_cmd = "GET_DIGITAL_VOLTAGE_OPTIONS";
        const string close_all_tabs_cmd = "CLOSE_ALL_TABS";
        const string exit = "EXIT";
        const string get_capture_range_cmd = "GET_CAPTURE_RANGE";
        const string get_viewstate_cmd = "GET_VIEWSTATE";
        const string set_viewstate_cmd = "SET_VIEWSTATE";

        public SaleaeClient(string host_str = "127.0.0.1", int port_input = 10429)
        {
            this.port = port_input;
            this.host = host_str;

            Socket = new TcpClient(host, port);
            Stream = Socket.GetStream();
        }

        private void WriteString(string str)
        {
            byte[] data = str.toByteArray().Concat("\0".toByteArray()).ToArray();

            Stream.Write(data, 0, data.Length);

            SaleaeStringHelper.WriteLine("Wrote data: " + str);
        }

        private void GetResponse(ref string response)
        {
            while ((string.IsNullOrEmpty(response)))
            {
                response += Stream.ReadString();
            }

            SaleaeStringHelper.WriteLine("Response: " + response);

            if (!(response.Substring(response.LastIndexOf('A')) == "ACK")) //note: this does not properly handle NAK replies.
            {
                throw new SaleaeSocketApiException();
            }
        }

        /// <summary>
        /// Give the Socket API a custom command
        /// </summary>
        /// <param name="export_command">Ex: "set_sample_rate, 10000000"</param>
        /// <returns>Response string</returns>
        public string CustomCommand(string export_command)
        {
            WriteString(export_command);

            string response = "";
            while ((string.IsNullOrEmpty(response)))
            {
                response += Stream.ReadString();
            }

            return response;
        }

        /// <summary>
        /// Set the capture trigger. Every active digital channel must be set, in order.
        /// To ignore the maximum_pulse_width_s parameter, set it to 0.
        /// </summary>
        /// <param name="triggers">List of triggers for active channels. Ex"High, Low, Posedge, Negedge, Low, High, ..."</param>
        public void SetTrigger(Trigger[] triggers, double minimum_pulse_width_s = 0.0, double maximum_pulse_width_s = 1.0)
        {
            List<string> command = new List<string>();

            command.Add(set_trigger_cmd);

            if (triggers.Count(x => x == Trigger.PositivePulse || x == Trigger.NegativePulse || x == Trigger.RisingEdge || x == Trigger.FallingEdge) > 1)
            {
                throw new SaleaeSocketApiException("invalid trigger specifications");
            }

            foreach (Trigger channel in triggers)
            {
                command.Add(channel.GetDescription());
                if (channel == Trigger.PositivePulse || channel == Trigger.NegativePulse)
                {
                    command.Add(minimum_pulse_width_s.ToString());
                    if (maximum_pulse_width_s > 0)
                        command.Add(maximum_pulse_width_s.ToString());
                }
            }

            string tx_command = string.Join(", ", command);
            WriteString(tx_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Get the number of samples to capture
        /// </summary>
        /// <returns></returns>
        public int GetNumSamples()
        {
            WriteString(get_num_samples_cmd);
            string response = "";
            GetResponse(ref response);

            return int.Parse(response.Split('\n').First());
        }

        /// <summary>
        /// Set number of samples for capture
        /// </summary>
        /// <param name="num_samples">Number of samples to set</param>
        public void SetNumSamples(int num_samples)
        {
            string export_command = set_num_samples_cmd + ", ";
            export_command += num_samples.ToString();
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Set number of seconds to capture for
        /// </summary>
        /// <param name="capture_seconds">Number of seconds to capture</param>
        public void SetCaptureSeconds(double seconds)
        {
            string export_command = set_capture_seconds_cmd + ", ";
            export_command += seconds.ToString();
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        public void SetDigitalVoltageOption(DigitalVoltageOption option)
        {
            string command = set_digital_voltage_option_cmd;
            command += ", " + option.Index.ToString();
            WriteString(command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Requires 1.2.6 or later software, otherwise it will throw
        /// </summary>
        /// <returns>List of possible IO threshold settings, if the active device supports multiple threshold voltages</returns>
        public List<DigitalVoltageOption>? GetDigitalVoltageOptions()
        {
            string command = get_digital_voltage_options_cmd;
            WriteString(command);

            string response = "";
            try
            {
                GetResponse(ref response);
            }
            catch (SaleaeSocketApiException)
            {
                //NAKed. No voltage options.
                return null;
            }

            var elems = response.Split(',', '\n').Select(x => x.Trim()).ToList();

            int voltage_count = elems.Count() / 3;
            List<DigitalVoltageOption> voltage_options = new List<DigitalVoltageOption>();

            for (int i = 0; i < voltage_count; ++i)
            {
                int index = i * 3;
                voltage_options.Add(new DigitalVoltageOption
                {
                    Index = int.Parse(elems[index]),
                    Description = elems[index + 1],
                    IsSelected = elems[index + 2].Contains("SELECTED") && !elems[index + 2].Contains("NOT_SELECTED")
                });

            }

            return voltage_options;
        }

        /// <summary>
        /// Closes all currently open tabs.
        /// </summary>
        public void CloseAllTabs()
        {
            string export_command = close_all_tabs_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Set the sample rate for capture
        /// </summary>
        /// <param name="sample_rate">Sample rate to set</param>
        public void SetSampleRate(SampleRate sample_rate)
        {
            string export_command = set_sample_rate_cmd + ", ";
            export_command += sample_rate.DigitalSampleRate.ToString();
            export_command += ", " + sample_rate.AnalogSampleRate.ToString();

            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Return the currently selected sample rate.
        /// </summary>
        /// <returns>Currently Selected Sample Rate</returns>
        public SampleRate GetSampleRate()
        {
            string command = get_sample_rate_cmd;
            WriteString(command);

            string response = "";
            GetResponse(ref response);

            var elems = response.Split('\n').Take(2).Select(x => int.Parse(x.Trim())).ToList();

            if (elems.Count() != 2)
            {
                throw new SaleaeSocketApiException("unexpected value");
            }

            return new SampleRate() { DigitalSampleRate = elems[0], AnalogSampleRate = elems[1] };
        }

        /// <summary>
        /// Start capture and save when capture finishes
        /// </summary>
        /// <param name="file">File to save capture to</param>
        public void CaptureToFile(string file)
        {
            string export_command = capture_to_file_cmd + ", ";
            export_command += file;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Save active tab capture to file
        /// </summary>
        /// <param name="file">File to save capture to</param>
        public void SaveToFile(string file)
        {
            string export_command = save_to_file_cmd + ", ";
            export_command += file;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Load a saved capture from fil
        /// </summary>
        /// <param name="file">File to load</param>
        public void LoadFromFile(string file)
        {
            string export_command = load_from_file_cmd + ", ";
            export_command += file;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        //create input struct
        public void ExportData(ExportDataStruct export_data_struct)
        {
            //channels
            const string all_channels_option = ", ALL_CHANNELS";
            const string digital_channels_option = ", DIGITAL_CHANNELS";
            const string analog_channels_option = ", ANALOG_CHANNELS";

            //time span
            const string all_time_option = ", ALL_TIME";
            const string time_span_option = ", TIME_SPAN";

            const string csv_option = ", CSV";
            const string headers_option = ", HEADERS";
            const string no_headers_option = ", NO_HEADERS";
            const string tab_option = ", TAB";
            const string comma_option = ", COMMA";
            const string sample_number_option = ", SAMPLE_NUMBER";
            const string time_stamp_option = ", TIME_STAMP";
            const string combined_option = ", COMBINED";
            const string separate_option = ", SEPARATE";
            const string row_per_change_option = ", ROW_PER_CHANGE";
            const string row_per_sample_option = ", ROW_PER_SAMPLE";
            const string dec_option = ", DEC";
            const string hex_option = ", HEX";
            const string bin_option = ", BIN";
            const string ascii_option = ", ASCII";

            const string binary_option = ", BINARY";
            const string each_sample_option = ", EACH_SAMPLE";
            const string on_change_option = ", ON_CHANGE";

            const string voltage_option = ", VOLTAGE";
            const string raw_adc_option = ", ADC";
            const string vcd_option = ", VCD";
            const string matlab_option = ", MATLAB";


            string export_command = export_data_cmd;
            export_command += ", " + export_data_struct.FileName;

            if (export_data_struct.ExportChannelSelection == DataExportChannelSelection.AllChannels)
                export_command += all_channels_option;
            else
            {
                if (export_data_struct.DigitalChannelsToExport.Length > 0)
                {
                    export_command += digital_channels_option;
                    foreach (int channel in export_data_struct.DigitalChannelsToExport)
                        export_command += ", " + channel.ToString();
                }

                if (export_data_struct.AnalogChannelsToExport.Length > 0)
                {
                    export_command += analog_channels_option;
                    foreach (int channel in export_data_struct.AnalogChannelsToExport)
                        export_command += ", " + channel.ToString();
                }
            }

            if ((export_data_struct.ExportChannelSelection == DataExportChannelSelection.AllChannels) || (export_data_struct.AnalogChannelsToExport != null && export_data_struct.AnalogChannelsToExport.Length > 0))
            {
                if (export_data_struct.AnalogFormat == AnalogOutputFormat.Voltage)
                    export_command += voltage_option;
                else if (export_data_struct.AnalogFormat == AnalogOutputFormat.ADC)
                    export_command += raw_adc_option;
            }

            if (export_data_struct.SamplesRangeType == DataExportSampleRangeType.RangeAll)
                export_command += all_time_option;
            else if (export_data_struct.SamplesRangeType == DataExportSampleRangeType.RangeTimes)
            {
                export_command += time_span_option;
                export_command += ", " + export_data_struct.StartingTime;
                export_command += ", " + export_data_struct.EndingTime;
            }

            if (export_data_struct.DataExportType == DataExportType.ExportCsv)
            {
                export_command += csv_option;

                if (export_data_struct.CsvIncludeHeaders == CsvHeadersType.CsvIncludesHeaders)
                    export_command += headers_option;
                else if (export_data_struct.CsvIncludeHeaders == CsvHeadersType.CsvNoHeaders)
                    export_command += no_headers_option;

                if (export_data_struct.CsvDelimiterType == CsvDelimiterType.CsvTab)
                    export_command += tab_option;
                else if (export_data_struct.CsvDelimiterType == CsvDelimiterType.CsvComma)
                    export_command += comma_option;

                if (export_data_struct.CsvTimestampType == CsvTimestampType.CsvSample)
                    export_command += sample_number_option;
                else if (export_data_struct.CsvTimestampType == CsvTimestampType.CsvTime)
                    export_command += time_stamp_option;

                if (export_data_struct.CsvOutputMode == CsvOutputMode.CsvSingleNumber)
                    export_command += combined_option;
                else if (export_data_struct.CsvOutputMode == CsvOutputMode.CsvOneColumnPerBit)
                    export_command += separate_option;

                if (export_data_struct.CsvDensity == CsvDensity.CsvTransition)
                    export_command += row_per_change_option;
                else if (export_data_struct.CsvDensity == CsvDensity.CsvComplete)
                    export_command += row_per_sample_option;

                if (export_data_struct.CsvDisplayBase == CsvBase.CsvDecimal)
                    export_command += dec_option;
                else if (export_data_struct.CsvDisplayBase == CsvBase.CsvHexadecimal)
                    export_command += hex_option;
                else if (export_data_struct.CsvDisplayBase == CsvBase.CsvBinary)
                    export_command += bin_option;
                else if (export_data_struct.CsvDisplayBase == CsvBase.CsvAscii)
                    export_command += ascii_option;
            }
            else if (export_data_struct.DataExportType == DataExportType.ExportBinary)
            {
                export_command += binary_option;

                if (export_data_struct.BinaryOutputMode == BinaryOutputMode.BinaryEverySample)
                    export_command += each_sample_option;
                else if (export_data_struct.BinaryOutputMode == BinaryOutputMode.BinaryEveryChange)
                    export_command += on_change_option;

                if (export_data_struct.BinaryOutputWordSize == BinaryOutputWordSize.Binary8Bit)
                    export_command += ", 8";
                else if (export_data_struct.BinaryOutputWordSize == BinaryOutputWordSize.Binary16Bit)
                    export_command += ", 16";
                else if (export_data_struct.BinaryOutputWordSize == BinaryOutputWordSize.Binary32Bit)
                    export_command += ", 32";
                else if (export_data_struct.BinaryOutputWordSize == BinaryOutputWordSize.Binary64Bit)
                    export_command += ", 64";

            }
            else if (export_data_struct.DataExportType == DataExportType.ExportVcd)
            {
                export_command += vcd_option;
            }
            else if (export_data_struct.DataExportType == DataExportType.ExportMatlab)
            {
                export_command += matlab_option;
            }

            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// This replaced the hard to use and buggy EXPORT_DATA command. The additional parameters, capture_contains_digital_channels and capture_contains_analog_channels, must be set if the capture itself contains channels of those types.
        /// This is because some export commands, such as digital-only exports and analog-only exports vary slightly if the source capture contains both digital and analog only channels.
        /// </summary>
        /// <param name="export_settings"></param>
        /// <param name="capture_contains_digital_channels">Set to true if the capture contains digital channels.</param>
        /// <param name="capture_contains_analog_channels">Set to true if the capture contains analog channels.</param>
        /// <returns></returns>
        public bool ExportData2(ExportDataStruct export_settings, bool capture_contains_digital_channels, bool capture_contains_analog_channels)
        {
            bool is_mixed_mode_capture = capture_contains_digital_channels && capture_contains_analog_channels; //different export options happen in this case.
            if (is_mixed_mode_capture && export_settings.ExportChannelSelection == DataExportChannelSelection.AllChannels)
                export_settings.DataExportMixedExportMode = DataExportMixedModeExportType.AnalogAndDigital; //this is not required to be explicitly set by the user.

            List<string> command_parts = new List<string>();
            command_parts.Add(export_data2_cmd);

            command_parts.Add(export_settings.FileName);

            command_parts.Add(export_settings.ExportChannelSelection.GetDescription());

            if (export_settings.ExportChannelSelection == DataExportChannelSelection.SpecificChannels)
            {
                if (is_mixed_mode_capture)
                    command_parts.Add(export_settings.DataExportMixedExportMode.GetDescription());

                if (export_settings.DigitalChannelsToExport != null && export_settings.DigitalChannelsToExport.Any())
                    command_parts.AddRange(export_settings.DigitalChannelsToExport.Select(x => new Channel { Index = x, DataType = Channel.ChannelDataType.DigitalChannel }.GetExportString()));
                if (export_settings.AnalogChannelsToExport != null && export_settings.AnalogChannelsToExport.Any())
                    command_parts.AddRange(export_settings.AnalogChannelsToExport.Select(x => new Channel { Index = x, DataType = Channel.ChannelDataType.AnalogChannel }.GetExportString()));
            }

            //time options.
            command_parts.Add(export_settings.SamplesRangeType.GetDescription());

            if (export_settings.SamplesRangeType == DataExportSampleRangeType.RangeTimes)
            {
                command_parts.Add(export_settings.StartingTime.ToString());
                command_parts.Add(export_settings.EndingTime.ToString());
            }

            command_parts.Add(export_settings.DataExportType.GetDescription());
            //digital only CSV
            if (capture_contains_digital_channels && export_settings.DataExportType == DataExportType.ExportCsv && (!is_mixed_mode_capture || export_settings.DataExportMixedExportMode == DataExportMixedModeExportType.DigitalOnly))
            {
                command_parts.Add(export_settings.CsvIncludeHeaders.GetDescription());
                command_parts.Add(export_settings.CsvDelimiterType.GetDescription());
                command_parts.Add(export_settings.CsvTimestampType.GetDescription());
                command_parts.Add(export_settings.CsvOutputMode.GetDescription());
                if (export_settings.CsvOutputMode == CsvOutputMode.CsvSingleNumber)
                    command_parts.Add(export_settings.CsvDisplayBase.GetDescription());
                command_parts.Add(export_settings.CsvDensity.GetDescription());
            }

            //analog only CSV
            if (capture_contains_analog_channels && export_settings.DataExportType == DataExportType.ExportCsv && (!is_mixed_mode_capture || export_settings.DataExportMixedExportMode == DataExportMixedModeExportType.AnalogOnly))
            {
                command_parts.Add(export_settings.CsvIncludeHeaders.GetDescription());
                command_parts.Add(export_settings.CsvDelimiterType.GetDescription());
                command_parts.Add(export_settings.CsvDisplayBase.GetDescription());
                command_parts.Add(export_settings.AnalogFormat.GetDescription());

            }

            //mixed mode CSV
            if (export_settings.DataExportType == DataExportType.ExportCsv && is_mixed_mode_capture && export_settings.DataExportMixedExportMode == DataExportMixedModeExportType.AnalogAndDigital)
            {
                command_parts.Add(export_settings.CsvIncludeHeaders.GetDescription());
                command_parts.Add(export_settings.CsvDelimiterType.GetDescription());
                command_parts.Add(export_settings.CsvDisplayBase.GetDescription());
                command_parts.Add(export_settings.AnalogFormat.GetDescription());
            }

            //digital binary
            if (capture_contains_digital_channels && export_settings.DataExportType == DataExportType.ExportBinary && (!is_mixed_mode_capture || export_settings.DataExportMixedExportMode == DataExportMixedModeExportType.DigitalOnly))
            {
                command_parts.Add(export_settings.BinaryOutputMode.GetDescription());
                command_parts.Add(export_settings.BinaryBitShifting.GetDescription());
                command_parts.Add(export_settings.BinaryOutputWordSize.GetDescription());
            }

            //analog only binary
            if (capture_contains_analog_channels && export_settings.DataExportType == DataExportType.ExportBinary && (!is_mixed_mode_capture || export_settings.DataExportMixedExportMode == DataExportMixedModeExportType.AnalogOnly))
            {
                command_parts.Add(export_settings.AnalogFormat.GetDescription());
            }

            //VCD (always digital only)
            if (export_settings.DataExportType == DataExportType.ExportVcd)
            {
                //no settings
            }

            //Matlab digital:
            if (capture_contains_digital_channels && export_settings.DataExportType == DataExportType.ExportMatlab && (!is_mixed_mode_capture || export_settings.DataExportMixedExportMode == DataExportMixedModeExportType.DigitalOnly))
            {
                //no settings
            }

            //Matlab analog or mixed:
            if (capture_contains_analog_channels && export_settings.DataExportType == DataExportType.ExportMatlab && (!is_mixed_mode_capture || export_settings.DataExportMixedExportMode != DataExportMixedModeExportType.DigitalOnly))
            {
                command_parts.Add(export_settings.AnalogFormat.GetDescription());
            }

            string socket_command = string.Join(", ", command_parts);
            WriteString(socket_command);

            string response = "";
            GetResponse(ref response);

            return true;
        }

        /// <summary>
        /// Get the active analyzers on the current tab
        /// </summary>
        /// <returns>A string of the names of the analyzers</returns>
        public List<Analyzer> GetAnalyzers()
        {
            string export_command = get_analyzers_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);

            var lines = response.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x) && !x.Contains("ACK")).ToList();

            List<Analyzer> analyzers = new List<Analyzer>();

            foreach (string line in lines)
            {
                var elements = line.Split(',').Select(x => x.Trim()).ToList();
                analyzers.Add(new Analyzer
                {
                    AnalyzerType = elements[0],
                    Index = int.Parse(elements[1])
                });
            }

            return analyzers;
        }

        /// <summary>
        /// Export a selected analyzer to a file
        /// </summary>
        /// <param name="selected">index of the selected analyzer(GetAnalyzer return string index + 1)</param>
        /// <param name="filename">file to save analyzer to</param>
        /// <param name="mXmitFile">mXmitFile</param>
        public void ExportAnalyzers(int selected, string filename, bool mXmitFile)
        {
            string export_command = export_analyzer_cmd + ", ";
            export_command += selected.ToString() + ", " + filename;
            if (mXmitFile == true)
                export_command += ", mXmitFile";
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
            if (mXmitFile == true)
                Console.WriteLine(response);
        }

        /// <summary>
        /// Start device capture
        /// </summary>
        public void Capture()
        {
            string export_command = capture_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Stop the current capture
        /// </summary>

        public void StopCapture()
        {
            string export_command = stop_capture_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Get size of pre-trigger buffer
        /// </summary>
        /// <returns>buffer size in # of samples</returns>
        public int GetCapturePretriggerBufferSize()
        {
            string export_command = get_capture_pretrigger_buffer_size_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
            string[] input_string = response.Split('\n');
            int buffer_size = int.Parse(input_string[0]);
            return buffer_size;
        }

        /// <summary>
        /// set pre-trigger buffer size
        /// </summary>
        /// <param name="buffer_size">buffer size in # of samples</param>
        public void SetCapturePretriggerBufferSize(int buffer_size)
        {
            string export_command = set_capture_pretrigger_buffer_size_cmd + ", ";
            export_command += buffer_size.ToString();
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Return the devices connected to the software
        /// </summary>
        /// <returns>Array of ConnectedDevices structs which contain device information</returns>
        public List<ConnectedDevice> GetConnectedDevices()
        {
            string command = get_connected_devices_cmd;
            WriteString(command);

            string response = "";
            GetResponse(ref response);
            var response_strings = response.Split('\n').ToList();
            response_strings.RemoveAll(x => x.Contains("ACK"));

            List<ConnectedDevice> devices = new List<ConnectedDevice>();

            foreach (string line in response_strings)
            {
                var elements = line.Split(',').Select(x => x.Trim()).ToList();

                DeviceType device_type;

                if (TryParseDeviceType(elements[2], out device_type) == false)
                    throw new SaleaeSocketApiException("unexpected value");

                ConnectedDevice device = new ConnectedDevice
                {
                    Index = int.Parse(elements[0]),
                    Name = elements[1],
                    DeviceType = device_type,
                    DeviceId = Convert.ToUInt64(elements[3].Substring(2), 16),
                    IsActive = (elements.Count() == 5 && elements[4] == "ACTIVE") ? true : false
                };

                devices.Add(device);
            }

            return devices;
        }

        /// <summary>
        /// Select the active capture device
        /// </summary>
        /// <param name="device_number">Index of device (as returned from ConnectedDevices struct)</param>
        public void SelectActiveDevice(int device_number)
        {
            string export_command = select_active_device_cmd + ", ";
            export_command += device_number.ToString();
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Set the performance option
        /// </summary>
        public void SetPerformanceOption(PerformanceOption performance)
        {
            string export_command = set_performance_cmd + ", ";
            export_command += performance.ToString("D");
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Get the performance option currently selected.
        /// </summary>
        /// <returns>A PerformanceOption enum</returns>
        public PerformanceOption GetPerformanceOption()
        {
            string export_command = get_performance_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);

            PerformanceOption selected_option = (PerformanceOption)Convert.ToInt32(response.Split('\n').First());
            return selected_option;
        }

        /// <summary>
        /// Get whether or not the software is done processing data. You must wait for data to be finished processing before you can export/save. 
        /// </summary>
        /// <returns>A boolean indicating if processing is complete</returns>
        public bool IsProcessingComplete()
        {
            string export_command = is_processing_complete_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);

            bool complete_processing = Convert.ToBoolean(response.Split('\n')[0]);
            return complete_processing;
        }

        /// <summary>
        /// Get whether or not the software is done processing data. You must wait for data to be finished processing before you can export/save. 
        /// </summary>
        /// <returns>A boolean indicating if processing is complete</returns>
        public bool IsAnalyzerProcessingComplete(int index)
        {
            string export_command = is_analyzer_complete_cmd;
            export_command += ", " + Convert.ToString(index);
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);

            bool complete_processing = Convert.ToBoolean(response.Split('\n')[0]);
            return complete_processing;
        }

        /// <summary>
        /// Calls IsProcessingComplete every 250 ms.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool BlockUntillProcessingCompleteOrTimeout(TimeSpan timeout)
        {
            DateTime processing_timeout = DateTime.Now.Add(timeout);
            bool processing_finished = false;
            do
            {

                processing_finished = IsProcessingComplete();

                if (!processing_finished)
                    System.Threading.Thread.Sleep(250);
            }
            while (!processing_finished && DateTime.Now < processing_timeout);

            return processing_finished;
        }

        /// <summary>
        /// Get the currently available sample rates for the selected performance options
        /// </summary>
        /// <returns>Array of sample rate combinations available</returns>
        public List<SampleRate> GetAvailableSampleRates()
        {
            WriteString(get_all_sample_rates_cmd);
            string response = "";
            GetResponse(ref response);

            List<SampleRate> sample_rates = new List<SampleRate>();
            string[] new_line = { "\n" };
            string[] responses = response.Split(new_line, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < responses.Length - 1; i++)
            {
                string[] split_sample_rate = responses[i].Split(',');
                if (split_sample_rate.Length != 2)
                {
                    sample_rates.Clear();
                    return sample_rates;
                }

                SampleRate new_sample_rate = new SampleRate();
                new_sample_rate.DigitalSampleRate = Convert.ToInt32(split_sample_rate[0].Trim());
                new_sample_rate.AnalogSampleRate = Convert.ToInt32(split_sample_rate[1].Trim());
                sample_rates.Add(new_sample_rate);
            }

            return sample_rates;
        }

        /// <summary>
        /// Get active channels for devices Logic16, Logic 8(second gen), Logic 8 pro, Logic 16 pro
        /// </summary>
        /// <returns>array of active channel numbers</returns>
        public void GetActiveChannels(List<int> digital_channels, List<int> analog_channels)
        {
            string export_command = get_active_channels_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);

            digital_channels.Clear();
            analog_channels.Clear();

            string[] input_string = response.Split('\n');
            string[] channels_string = input_string[0].Split(',').Select(x => x.Trim()).ToArray();

            bool add_to_digital_channel_list = true;
            for (int i = 0; i < channels_string.Length; ++i)
            {
                if (channels_string[i] == "digital_channels")
                {
                    add_to_digital_channel_list = true;
                    continue;
                }
                else if (channels_string[i] == "analog_channels")
                {
                    add_to_digital_channel_list = false;
                    continue;
                }

                if (add_to_digital_channel_list)
                    digital_channels.Add(int.Parse(channels_string[i]));
                else
                    analog_channels.Add(int.Parse(channels_string[i]));
            }
        }

        /// <summary>
        /// Set the active channels for devices Logic16, Logic 8(second gen), Logic 8 pro, Logic 16 pro
        /// </summary>
        /// <param name="channels">array of channels to be active: 0-15</param>
        public void SetActiveChannels(int[]? digital_channels = null, int[]? analog_channels = null)
        {
            string export_command = set_active_channels_cmd;
            if (digital_channels != null)
            {
                export_command += ", " + "digital_channels";

                for (int i = 0; i < digital_channels.Length; ++i)
                {
                    export_command += ", " + digital_channels[i].ToString();
                }
            }

            if (analog_channels != null)
            {
                export_command += ", " + "analog_channels";

                for (int i = 0; i < analog_channels.Length; ++i)
                {
                    export_command += ", " + analog_channels[i].ToString();
                }
            }

            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Reset to default active logic 16 channels (0-15)
        /// </summary>
        public void ResetActiveChannels()
        {
            string export_command = reset_active_channels_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Close the Logic software. this will close the socket.
        /// </summary>
        public void Exit()
        {
            WriteString(exit);
        }

        /// <summary>
        /// Returns the index of the first valid sample (0, unless a trigger was used)
        /// The trigger Sample (0, unless a trigger was used)
        /// The ending sample.
        /// The LCM Sample Rate (least common multiple)
        /// Sample indexes are in LCM samples, which is the least common multiple of the digital sample rate and the analog sample rate
        /// The requires Logic software version 1.2.18 or greater.
        /// </summary>
        /// <returns></returns>
        public CaptureRange GetCaptureRange()
        {
            string export_command = get_capture_range_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);

            //remove trailing ACK
            if (!response.EndsWith("ACK"))
            {
                throw new SaleaeSocketApiException("invalid reponse");
            }

            response = response.Remove(response.LastIndexOf("ACK"));

            string[] input_string = response.Split(',');

            return new CaptureRange
            {
                StartingSample = UInt64.Parse(input_string[0]),
                TriggerSample = UInt64.Parse(input_string[1]),
                EndingSample = UInt64.Parse(input_string[2]),
                LcmSampleRate = UInt32.Parse(input_string[3])
            };
        }

        /// <summary>
        /// Returns the current viewstate information of the open tab. 
        /// The requires Logic software version 1.2.18 or greater.
        /// </summary>
        /// <returns></returns>
        public ViewState GetViewState()
        {
            string export_command = get_viewstate_cmd;
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);

            //remove trailing ACK
            if (!response.EndsWith("ACK"))
            {
                throw new SaleaeSocketApiException("invalid reponse");
            }

            response = response.Remove(response.LastIndexOf("ACK"));

            string[] input_string = response.Split(',');

            return new ViewState
            {
                ZoomSamplesPerPixel = double.Parse(input_string[0]),
                PanStartingSample = double.Parse(input_string[1]),
                LcmSampleRate = UInt32.Parse(input_string[2])
            };
        }

        /// <summary>
        /// Sets the viewstate of the open tab.
        /// The requires Logic software version 1.2.18 or greater.
        /// </summary>
        /// <param name="zoom_samples_per_pixel">The zoom level, in samples per pixel. larger is more zoomed out. 1 represents 1 sample per pixel. (samples are realitve to the LCM rate)</param>
        /// <param name="pan_starting_sample">The sample index of the left edge of the display, relative to the LCM sample rate</param>
        public void SetViewState(double zoom_samples_per_pixel, double pan_starting_sample)
        {
            string export_command = string.Format("{0}, {1:f}, {2:f}", set_viewstate_cmd, zoom_samples_per_pixel, pan_starting_sample);
            WriteString(export_command);

            string response = "";
            GetResponse(ref response);
        }

        private bool TryParseDeviceType(string input, out DeviceType device_type)
        {
            device_type = DeviceType.Logic; // default.

            var all_options = Enum.GetValues(typeof(DeviceType)).Cast<DeviceType>();

            if (all_options.Any(x => x.GetDescription() == input.Trim()))
            {
                device_type = all_options.Single(x => x.GetDescription() == input.Trim());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
