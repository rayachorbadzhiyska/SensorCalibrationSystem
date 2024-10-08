﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorCalibrationSystem.Contracts;
using SensorCalibrationSystem.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text.Json;
using System.Windows;

namespace SensorCalibrationSystem.ViewModels
{
    public class MemoryMapViewModel : ObservableObject, INavigationPage, IReloadable
    {
        #region Properties Interfaces

        public string Name { get; set; } = "Memory Map";

        public string Logo { get; set; } = "/Resources/memory-map.png";

        public int NavigationIndex { get; set; }

        public bool IsActive { get; set; }

        public bool HasBeenLoaded { get; set; }

        #endregion

        #region Fields

        private readonly string sensorMemoryMapDataPath = @"memory-maps";
        private readonly string communicationTypesFilePath = @"Resources\communication-types.json";

        private readonly IBoardCommunicationService boardCommunicationService;

        private MemoryMapModel? selectedMemoryMap;
        private string? selectedValueFormat;

        private List<string> communicationTypes;
        private string selectedCommunicationType;

        #endregion

        #region Properties

        public ObservableCollection<MemoryMapModel> MemoryMaps { get; set; }

        public MemoryMapModel? SelectedMemoryMap
        {
            get => selectedMemoryMap;
            set => SetProperty(ref selectedMemoryMap, value);
        }

        public List<string> ValueFormats { get; } = new List<string>
        {
            "Decimal",
            "Hex",
            "Binary"
        };

        public string? SelectedValueFormat
        {
            get => selectedValueFormat;
            set => SetProperty(ref selectedValueFormat, value);
        }

        public List<string> CommunicationTypes
        {
            get => communicationTypes;
            set => SetProperty(ref communicationTypes, value);
        }

        public string SelectedCommunicationType
        {
            get => selectedCommunicationType;
            set => SetProperty(ref selectedCommunicationType, value);
        }

        #endregion

        #region Commands

        public IRelayCommand<RegisterModel> ReadRegisterCommand { get; }

        public IRelayCommand<RegisterModel> WriteRegisterCommand { get; }

        #endregion

        #region Constructor

        public MemoryMapViewModel(IBoardCommunicationService boardCommunicationService)
        {
            this.boardCommunicationService = boardCommunicationService;

            ReadRegisterCommand = new RelayCommand<RegisterModel>(ReadRegister);
            WriteRegisterCommand = new RelayCommand<RegisterModel>(WriteRegister);

            MemoryMaps = new ObservableCollection<MemoryMapModel>();
        }

        #endregion

        #region Methods

        private void LoadMemoryMapData()
        {
            string[] files = Directory.GetFiles(sensorMemoryMapDataPath);

            foreach (var file in files)
            {
                string jsonData = File.ReadAllText(file);

                MemoryMapModel? memoryMap = JsonSerializer.Deserialize<MemoryMapModel>(jsonData);

                if (memoryMap is not null)
                {
                    MemoryMaps.Add(memoryMap);
                }
            }
        }

        private void LoadCommunicationTypes()
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullCommunicationTypesFilePath = Path.Combine(directoryPath, communicationTypesFilePath);

            if (File.Exists(fullCommunicationTypesFilePath))
            {
                string? jsonData = File.ReadAllText(communicationTypesFilePath);

                if (string.IsNullOrEmpty(jsonData))
                {
                    MessageBox.Show("Communication types file is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                List<string> communicationTypesList = JsonSerializer.Deserialize<List<string>>(jsonData);

                if (communicationTypesList is not null && communicationTypesList.Any())
                {
                    CommunicationTypes = communicationTypesList;
                    SelectedCommunicationType = CommunicationTypes.First();
                }
            }
            else
            {
                MessageBox.Show("Communication types file does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BoardCommunicationService_SerialDataReceived(object? sender, string e)
        {
            if (e.StartsWith("OK Register READ"))
            {
                string[] data = e.Split(new char[] { ' ', ':' }, System.StringSplitOptions.RemoveEmptyEntries); // e.g. OK Register READ 0x2B: 255
                if (data.Length == 5)
                {
                    string regAddress = data[3];
                    int regValue = int.Parse(data[4]);

                    if (SelectedMemoryMap is not null
                        && SelectedMemoryMap.Registers is not null
                        && SelectedMemoryMap.Registers.Any(x => x.Address == regAddress))
                    {
                        SelectedMemoryMap.Registers.First(x => x.Address == regAddress).Value = regValue.ToString();
                    }
                }
            }
            else if (e.StartsWith("ERROR:"))
            {
                string[] data = e.Split(':', System.StringSplitOptions.RemoveEmptyEntries);
                string errorMessage = data[1];
                MessageBox.Show(errorMessage, "An error has occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadRegister(RegisterModel? register)
        {
            if (register is RegisterModel)
            {
                boardCommunicationService.WriteLine($"READ {register.Address} {register.Size}");
            }
        }

        private void WriteRegister(RegisterModel? register)
        {
            if (register is RegisterModel)
            {
                boardCommunicationService.WriteLine($"WRITE {register.Address} {register.Value} {register.Size}");
            }
        }

        #endregion

        #region Navigation Methods

        public void OnNavigatedTo()
        {
            if (!HasBeenLoaded)
            {
                LoadMemoryMapData();

                LoadCommunicationTypes();

                SelectedMemoryMap = MemoryMaps.FirstOrDefault();
                SelectedValueFormat = ValueFormats.First();

                HasBeenLoaded = true;
            }

            boardCommunicationService.SerialDataReceived += BoardCommunicationService_SerialDataReceived;
        }

        public void OnNavigatedFrom()
        {
            boardCommunicationService.SerialDataReceived -= BoardCommunicationService_SerialDataReceived;
        }

        #endregion
    }
}
