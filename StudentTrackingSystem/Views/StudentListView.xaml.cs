#region Kütüphane Tanýmlamalarý
using StudentTrackingSystem.Models;
using StudentTrackingSystem.Services;
using StudentTrackingSystem.ViewModels;
#endregion

namespace StudentTrackingSystem.Views;

public partial class StudentListView : ContentPage
{
    #region Özel Deðiþkenler ve Servisler
    private readonly StudentService _studentService;
    private int _classId;
    private List<StudentViewModel> _studentViewModels;
    #endregion

    #region Yapýcý Metot (Constructor)
    public StudentListView(int classId, string className)
    {
        try
        {
            InitializeComponent();
            _studentService = new StudentService();
            _classId = classId;
            LblClassName.Text = className;

            // Veri yüklemeyi tetikle
            LoadStudents();
        }
        catch { /**/ }
    }
    #endregion

    #region Veri Yükleme Ýþlemleri
    private async void LoadStudents()
    {
        try
        {
            var students = await _studentService.GetStudentsByClassIdAsync(_classId);
            if (students == null) return;

            _studentViewModels = students.Select(s => new StudentViewModel
            {
                StudentData = s,
                SelectedStatusId = 1 // Varsayýlan: Geldi
            }).ToList();

            StudentCollection.ItemsSource = _studentViewModels;
        }
        catch { /**/ }
    }
    #endregion

    #region Kullanýcý Etkileþimleri
    private async void OnPeriodChanged(object sender, EventArgs e)
    {
        try
        {
            if (PeriodPicker.SelectedIndex == -1) return;
            int lessonNumber = PeriodPicker.SelectedIndex + 1;

            // O ders saatine ait kayýtlý yoklama var mý?
            var existingAttendance = await _studentService.GetExistingAttendanceAsync(_classId, lessonNumber);
            bool hasData = existingAttendance != null && existingAttendance.Count > 0;

            // Arayüz kontrollerini güncelle
            StatusWarningFrame.IsVisible = hasData;
            BtnSave.IsVisible = !hasData;
            BtnUpdate.IsVisible = hasData;

            if (_studentViewModels != null)
            {
                foreach (var vm in _studentViewModels)
                {
                    // Eðer veritabanýnda bu öðrenci için o ders saatinde kayýt varsa onu getir, yoksa 'Geldi' yap
                    if (hasData && existingAttendance.TryGetValue(vm.StudentData.Id, out int statusId))
                        vm.SelectedStatusId = statusId;
                    else
                        vm.SelectedStatusId = 1;
                }
            }
        }
        catch { /**/ }
    }

    private async void OnSaveAttendanceClicked(object sender, EventArgs e)
    {
        await ProcessAttendance(isUpdate: false);
    }

    private async void OnUpdateAttendanceClicked(object sender, EventArgs e)
    {
        // Güncelleme butonu týklandýðýnda onay alarak iþlemi baþlatýr
        bool confirm = await DisplayAlert("Onay", "Mevcut yoklama kaydýný deðiþtirmek istediðinize emin misiniz?", "Evet", "Hayýr");
        if (confirm)
        {
            await ProcessAttendance(isUpdate: true);
        }
    }

    /// <summary>
    /// Hem Kaydet hem de Güncelleme iþlemini yöneten merkezi metot
    /// </summary>
    private async Task ProcessAttendance(bool isUpdate)
    {
        try
        {
            if (PeriodPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Uyarý", "Lütfen önce ders saatini seçiniz!", "Tamam");
                return;
            }

            int lessonNumber = PeriodPicker.SelectedIndex + 1;
            var attendanceData = _studentViewModels
                .Select(vm => (vm.StudentData.Id, vm.SelectedStatusId))
                .ToList();

            // Kayýt veya Güncelleme iþlemini gerçekleþtir
            await _studentService.SaveBulkAttendanceAsync(attendanceData, _classId, 1, lessonNumber);

            string message = isUpdate ? "Yoklama güncellendi." : "Yoklama baþarýyla kaydedildi.";
            await DisplayAlert("Bilgi", message, "Tamam");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Ýþlem sýrasýnda sorun çýktý: {ex.Message}", "Tamam");
        }
    }

    private void OnStatusTapped(object sender, TappedEventArgs e)
    {
        try
        {
            if (sender is Border border && border.BindingContext is StudentViewModel vm && e.Parameter != null)
            {
                if (int.TryParse(e.Parameter.ToString(), out int statusId))
                {
                    vm.SelectedStatusId = statusId;
                }
            }
        }
        catch { /**/ }
    }

    private async void OnStudentDetailClicked(object sender, EventArgs e)
    {
        try
        {
            var visualElement = sender as VisualElement;
            var selectedStudent = visualElement?.BindingContext as StudentViewModel;

            if (selectedStudent?.StudentData != null)
            {
                await Navigation.PushAsync(new StudentDetailView(selectedStudent.StudentData.Id));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Öðrenci detaylarý yüklenemedi: " + ex.Message, "Tamam");
        }
    }
    #endregion
}