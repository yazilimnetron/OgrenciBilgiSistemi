#region K�t�phane Tan�mlamalar�
using StudentTrackingSystem.Models;
using StudentTrackingSystem.Services;
using StudentTrackingSystem.ViewModels;
#endregion

namespace StudentTrackingSystem.Views;

public partial class StudentListView : ContentPage
{
    #region �zel De�i�kenler ve Servisler
    private readonly StudentService _studentService;
    private int _classId;
    private List<StudentViewModel> _studentViewModels;
    #endregion

    #region Yap�c� Metot (Constructor)
    public StudentListView(int classId, string className)
    {
        try
        {
            InitializeComponent();
            _studentService = new StudentService();
            _classId = classId;
            LblClassName.Text = className;

            // Veri y�klemeyi tetikle
            LoadStudents();
        }
        catch { /**/ }
    }
    #endregion

    #region Veri Y�kleme ��lemleri
    private async void LoadStudents()
    {
        try
        {
            var students = await _studentService.GetStudentsByClassIdAsync(_classId);
            if (students == null) return;

            _studentViewModels = students.Select(s => new StudentViewModel
            {
                StudentData = s,
                SelectedStatusId = 1 // Varsay�lan: Geldi
            }).ToList();

            StudentCollection.ItemsSource = _studentViewModels;
        }
        catch { /**/ }
    }
    #endregion

    #region Kullan�c� Etkile�imleri
    private async void OnPeriodChanged(object sender, EventArgs e)
    {
        try
        {
            if (PeriodPicker.SelectedIndex == -1) return;
            int lessonNumber = PeriodPicker.SelectedIndex + 1;

            // O ders saatine ait kay�tl� yoklama var m�?
            var existingAttendance = await _studentService.GetExistingAttendanceAsync(_classId, lessonNumber);
            bool hasData = existingAttendance != null && existingAttendance.Count > 0;

            // Aray�z kontrollerini g�ncelle
            StatusWarningFrame.IsVisible = hasData;
            BtnSave.IsVisible = !hasData;
            BtnUpdate.IsVisible = hasData;

            if (_studentViewModels != null)
            {
                foreach (var vm in _studentViewModels)
                {
                    // E�er veritaban�nda bu ��renci i�in o ders saatinde kay�t varsa onu getir, yoksa 'Geldi' yap
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
        // G�ncelleme butonu t�kland���nda onay alarak i�lemi ba�lat�r
        bool confirm = await DisplayAlert("Onay", "Mevcut yoklama kayd�n� de�i�tirmek istedi�inize emin misiniz?", "Evet", "Hay�r");
        if (confirm)
        {
            await ProcessAttendance(isUpdate: true);
        }
    }

    /// <summary>
    /// Hem Kaydet hem de G�ncelleme i�lemini y�neten merkezi metot
    /// </summary>
    private async Task ProcessAttendance(bool isUpdate)
    {
        try
        {
            if (PeriodPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Uyar�", "L�tfen �nce ders saatini se�iniz!", "Tamam");
                return;
            }

            int lessonNumber = PeriodPicker.SelectedIndex + 1;
            var attendanceData = _studentViewModels
                .Select(vm => (vm.StudentData.Id, vm.SelectedStatusId))
                .ToList();

            // Kay�t veya G�ncelleme i�lemini ger�ekle�tir
            // Giriş yapan öğretmenin ID'si UserSession üzerinden alınır
            await _studentService.SaveBulkAttendanceAsync(attendanceData, _classId, UserSession.UserId, lessonNumber);

            string message = isUpdate ? "Yoklama g�ncellendi." : "Yoklama ba�ar�yla kaydedildi.";
            await DisplayAlert("Bilgi", message, "Tamam");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"��lem s�ras�nda sorun ��kt�: {ex.Message}", "Tamam");
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
            await DisplayAlert("Hata", "��renci detaylar� y�klenemedi: " + ex.Message, "Tamam");
        }
    }
    #endregion
}