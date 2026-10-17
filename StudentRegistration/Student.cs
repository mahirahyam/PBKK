using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace StudentRegistration
{
    // --- MODEL ---
    public class Student
    {
        public string NIM { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string ProgramStudi { get; set; } = string.Empty;
        public string JenisKelamin { get; set; } = string.Empty;
        public DateTime TanggalLahir { get; set; } = DateTime.Now;
        public string Alamat { get; set; } = string.Empty;
        public string NomorTelepon { get; set; } = string.Empty;
    }

    // --- VIEWMODEL ---
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Student> _students = new();
        private ObservableCollection<Student> _filteredStudents = new();
        private Student? _selectedStudent;
        private string _searchNIM = string.Empty;

        private string _nim = string.Empty;
        private string _nama = string.Empty;
        private string _programStudi = string.Empty;
        private string _jenisKelamin = "Laki-laki";
        private DateTime _tanggalLahir = DateTime.Now;
        private string _alamat = string.Empty;
        private string _nomorTelepon = string.Empty;
        private bool _isEditing = false;

        public ObservableCollection<Student> FilteredStudents
        {
            get => _filteredStudents;
            set { _filteredStudents = value; OnPropertyChanged(nameof(FilteredStudents)); }
        }

        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));
                if (_selectedStudent != null) LoadSelectedStudent();
            }
        }

        public string NIM { get => _nim; set { _nim = value; OnPropertyChanged(nameof(NIM)); } }
        public string Nama { get => _nama; set { _nama = value; OnPropertyChanged(nameof(Nama)); } }
        public string ProgramStudi { get => _programStudi; set { _programStudi = value; OnPropertyChanged(nameof(ProgramStudi)); } }
        public string JenisKelamin { get => _jenisKelamin; set { _jenisKelamin = value; OnPropertyChanged(nameof(JenisKelamin)); } }
        public DateTime TanggalLahir { get => _tanggalLahir; set { _tanggalLahir = value; OnPropertyChanged(nameof(TanggalLahir)); } }
        public string Alamat { get => _alamat; set { _alamat = value; OnPropertyChanged(nameof(Alamat)); } }
        public string NomorTelepon { get => _nomorTelepon; set { _nomorTelepon = value; OnPropertyChanged(nameof(NomorTelepon)); } }
        public string SearchNIM { get => _searchNIM; set { _searchNIM = value; OnPropertyChanged(nameof(SearchNIM)); Search(); } }
        public bool IsEditing { get => _isEditing; set { _isEditing = value; OnPropertyChanged(nameof(IsEditing)); } }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ResetCommand { get; }

        public MainViewModel()
        {
            SaveCommand = new RelayCommand(SaveStudent);
            DeleteCommand = new RelayCommand(DeleteStudent, () => SelectedStudent != null);
            ResetCommand = new RelayCommand(ResetForm);
            FilteredStudents = _students;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(NIM) || string.IsNullOrWhiteSpace(Nama) ||
                string.IsNullOrWhiteSpace(ProgramStudi) || string.IsNullOrWhiteSpace(NomorTelepon))
            {
                MessageBox.Show("NIM, Nama, Program Studi, dan Nomor Telepon wajib diisi!", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void SaveStudent()
        {
            if (!ValidateInput()) return;

            if (IsEditing)
            {
                var student = _students.FirstOrDefault(s => s.NIM == NIM);
                if (student != null)
                {
                    student.Nama = Nama;
                    student.ProgramStudi = ProgramStudi;
                    student.JenisKelamin = JenisKelamin;
                    student.TanggalLahir = TanggalLahir;
                    student.Alamat = Alamat;
                    student.NomorTelepon = NomorTelepon;
                    MessageBox.Show("Data mahasiswa berhasil diperbarui!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                if (_students.Any(s => s.NIM == NIM))
                {
                    MessageBox.Show("NIM sudah terdaftar!", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _students.Add(new Student
                {
                    NIM = NIM,
                    Nama = Nama,
                    ProgramStudi = ProgramStudi,
                    JenisKelamin = JenisKelamin,
                    TanggalLahir = TanggalLahir,
                    Alamat = Alamat,
                    NomorTelepon = NomorTelepon
                });
                MessageBox.Show("Data mahasiswa berhasil ditambahkan!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ResetForm();
        }

        private void DeleteStudent()
        {
            if (SelectedStudent == null) return;

            var result = MessageBox.Show($"Apakah Anda yakin ingin menghapus {SelectedStudent.Nama}?", "Konfirmasi Hapus", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _students.Remove(SelectedStudent);
                ResetForm();
            }
        }

        private void LoadSelectedStudent()
        {
            if (SelectedStudent == null) return;

            NIM = SelectedStudent.NIM;
            Nama = SelectedStudent.Nama;
            ProgramStudi = SelectedStudent.ProgramStudi;
            JenisKelamin = SelectedStudent.JenisKelamin;
            TanggalLahir = SelectedStudent.TanggalLahir;
            Alamat = SelectedStudent.Alamat;
            NomorTelepon = SelectedStudent.NomorTelepon;
            IsEditing = true;
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchNIM))
            {
                FilteredStudents = _students;
            }
            else
            {
                FilteredStudents = new ObservableCollection<Student>(_students.Where(s => s.NIM.Contains(SearchNIM, StringComparison.OrdinalIgnoreCase)));
            }
        }

        private void ResetForm()
        {
            NIM = string.Empty;
            Nama = string.Empty;
            ProgramStudi = string.Empty;
            JenisKelamin = "Laki-laki";
            TanggalLahir = DateTime.Now;
            Alamat = string.Empty;
            NomorTelepon = string.Empty;
            SelectedStudent = null;
            IsEditing = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // --- RELAY COMMAND ---
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    // --- CONVERTERS ---
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b && b ? parameter?.ToString() ?? Binding.DoNothing : Binding.DoNothing;
        }
    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b && !b;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b && !b;
        }
    }
}