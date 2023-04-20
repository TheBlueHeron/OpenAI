using System.Diagnostics;
using Windows.System;

namespace BlueHeron.OpenAI.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ServiceConnection _openai = new();
    [ObservableProperty()]
    private string _answer = string.Empty;
    [ObservableProperty()]
    private string _question = string.Empty;

    public MainViewModel()
    {
        Title = "Home";
    }


    [RelayCommand]
    private async void AnswerQuestion()
    {
        Answer = string.Empty;

        await foreach (var t in _openai.Answer(Question))
        {
            _ = await UpdateAnswer(t);
        }
    }

    private async Task<bool> UpdateAnswer(string t)
    {
        await Task.Run(() =>
        {
            Answer += t;
        });
        return true;
    }
}
