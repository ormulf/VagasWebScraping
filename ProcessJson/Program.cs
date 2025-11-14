using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Encodings.Web;

var pontuations = new List<string> { ".", ",",":",";", "?", "!", "'", "\"", "(", ")", "/", "-", "+", "*" };
var wordsToIgnorePT = new List<string> {};
var wordsToIgnoreEN = new List<string> {};
var keyWords = new List<string> {};

wordsToIgnorePT = ReadJsonStrings("C:\\Users\\ormul\\source\\repos\\jsonWordsToIgnorePT.json");
wordsToIgnoreEN = ReadJsonStrings("C:\\Users\\ormul\\source\\repos\\jsonWordsToIgnoreEN.json");
keyWords = ReadJsonStrings("C:\\Users\\ormul\\source\\repos\\keyWords.json");

Console.WriteLine("Process Json jobs?");
var answer = Console.ReadLine();
if (answer == "y")
{
    string caminhoArquivo = "C:\\Users\\ormul\\source\\repos\\Gerenteundefinedde engenharia de software.json";
    JobData jobs = ReadJsonJobs(caminhoArquivo);
    var wordsToClassifyList = new List<WordsToClassify>();
    //ReadJsonStrings
   
    //-
    foreach (var job in jobs.JobFromJson.Skip(56).Take(1))
    {
        Console.WriteLine("---------------------------------------------------");
        job.PrintJobInfo();
        var language = IdentifyLanguage(job.JobDescription);
        Console.WriteLine(language);
        Console.WriteLine("Language is correct?");
        answer = Console.ReadLine();
        if (answer != string.Empty)
            language = answer.ToUpper();

        wordsToIgnorePT = ReadJsonStrings("C:\\Users\\ormul\\source\\repos\\jsonWordsToIgnorePT.json");
        wordsToIgnoreEN = ReadJsonStrings("C:\\Users\\ormul\\source\\repos\\jsonWordsToIgnoreEN.json");
        keyWords = ReadJsonStrings("C:\\Users\\ormul\\source\\repos\\keyWords.json");

        var fromExistingKeyWords = GetExistingKeyWords(job.JobDescription);
        Console.WriteLine("Existing KeyWords in Job Description: " + string.Join(", ", fromExistingKeyWords));
        var words = job.JobDescription.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        words = RemovePontuations(words);
        words = RemoveWordsToIgnore(words, language == "PT" ? wordsToIgnorePT : wordsToIgnoreEN);
        words = RemoveNumbers(words);
        words = RemoveExistingKeyWords(words, fromExistingKeyWords);

        wordsToClassifyList.AddRange(ConvertWordsIntoWordsToClassify(words.Distinct().ToArray(), language));
        Console.WriteLine("New Words to Classify: " + wordsToClassifyList.Count.ToString());
        Console.WriteLine("---------------------------------------------------\n");
    }
    var options = new JsonSerializerOptions
    {
        WriteIndented = true, // opcional
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    var jsonWordsToClassify = JsonSerializer.Serialize(wordsToClassifyList, options);
    File.WriteAllText("C:\\Users\\ormul\\source\\repos\\wordsToClassify.json", jsonWordsToClassify.ToLower(), System.Text.Encoding.UTF8);
}
Console.WriteLine("Process Json WordsToClassify?");
answer = Console.ReadLine();
if (answer == "y")
{
    var path = "C:\\Users\\ormul\\source\\repos\\wordsToClassify.json";
    wordsToIgnorePT = ReadJsonStrings("C:\\Users\\ormul\\source\\repos\\jsonWordsToIgnorePT.json");
    wordsToIgnoreEN = ReadJsonStrings("C:\\Users\\ormul\\source\\repos\\jsonWordsToIgnoreEN.json");
    keyWords = ReadJsonStrings("C:\\Users\\ormul\\source\\repos\\keyWords.json");
    var wordsToClassify = new List<WordsToClassify>();
    using (var arquivo = File.OpenRead(path))
    {
        // Desserializa o JSON em uma lista de objetos Pessoa
        wordsToClassify = JsonSerializer.Deserialize<List<WordsToClassify>>(arquivo);        
    }
    keyWords.AddRange(wordsToClassify.Where(w => w.IgnoreList == false).Select(w => w.Word).ToList());
    var wordsToIgnore = wordsToClassify.Where(w => w.IgnoreList == true).Select(w => w.Word).ToList();
    if (wordsToIgnore.Count > 0)
    {
        if (wordsToClassify.First().Language.ToUpper() == "PT")
        {
            wordsToIgnorePT.AddRange(wordsToIgnore);
        }
        else
        {
            wordsToIgnoreEN.AddRange(wordsToIgnore);
        }
    }
    
    var options = new JsonSerializerOptions
    {
        WriteIndented = true, // opcional
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    var jsonWordsToIgnorePT = JsonSerializer.Serialize(wordsToIgnorePT.Where(w=> w is not null).ToList(), options);
    var jsonWordsToIgnoreEN = JsonSerializer.Serialize(wordsToIgnoreEN.Where(w => w is not null).ToList(), options);
    var jsonKeyWords = JsonSerializer.Serialize(keyWords.Where(w => w is not null).ToList(), options);
    File.WriteAllText("C:\\Users\\ormul\\source\\repos\\jsonWordsToIgnorePT.json", jsonWordsToIgnorePT.ToLower(), System.Text.Encoding.UTF8);
    File.WriteAllText("C:\\Users\\ormul\\source\\repos\\jsonWordsToIgnoreEN.json", jsonWordsToIgnoreEN.ToLower(), System.Text.Encoding.UTF8);
    File.WriteAllText("C:\\Users\\ormul\\source\\repos\\keyWords.json", jsonKeyWords.ToLower(), System.Text.Encoding.UTF8);
}

string[] RemoveExistingKeyWords(string[] words, List<string> existingKeyWords)
{
    var result = words;
    foreach (var kw in existingKeyWords)
    {
        result = result.Where(word => word.ToLower() != kw.ToLower()).ToArray();
    }
    return result;
}
List<string> GetExistingKeyWords(string text)
{
    var ContainsThisKeyWords = new List<string> { };
    foreach (var keyWord in keyWords)
    {
        if (text.Contains(keyWord, StringComparison.OrdinalIgnoreCase))
        {
            ContainsThisKeyWords.Add(keyWord);
        }
    }
    return ContainsThisKeyWords;
}
List<string> ReadJsonStrings(string path)
{
    using (var arquivo = File.OpenRead(path))
    {
        // Desserializa o JSON em uma lista de objetos Pessoa
        var words = JsonSerializer.Deserialize<List<string>>(arquivo);
        return words;
    }
}

JobData ReadJsonJobs(string path)
{
    using (var arquivo = File.OpenRead(path))
    {
        // Desserializa o JSON em uma lista de objetos Pessoa
        var jobs = JsonSerializer.Deserialize<JobData>(arquivo);
        return jobs;
    }
}
List<WordsToClassify> ConvertWordsIntoWordsToClassify(string[] words,string language)
{
    var wordsToClassify = new List<WordsToClassify>();
    foreach (var word in words)
    {
        var lowerWord = word.ToLower();
        wordsToClassify.Add(new WordsToClassify
        {
            Word = lowerWord,
            Language = language,
            Compound = false,
            IgnoreList = true
        });
    }
    return wordsToClassify;
}

string[] RemoveNumbers(string[] words)
{
    return words.Where(word => !int.TryParse(word, out _)).ToArray();
}
string[] RemoveWordsToIgnore(string[] words, List<string> wordsToIgnore)
{
    var result = words;
    foreach (var wordToIgnore in wordsToIgnore)
    {
        result = result.Where(word => word.ToLower() != wordToIgnore.ToLower()).ToArray();
    }
    return result;
}
string[] RemovePontuations(string[] words)
{
    for (int i = 0; i < words.Length; i++)
    {
        var lastChar = words[i][words[i].Length - 1].ToString();
        var firstChar = words[i][0].ToString();
        if (words[i].Length > 1)
        {
            if (pontuations.Contains(lastChar))
            {
                words[i] = words[i].Substring(0, words[i].Length - 1);
            }
            if (pontuations.Contains(firstChar))
            {
                words[i] = words[i].Substring(1, words[i].Length - 1);
            }
        }
        else
        {
            if (pontuations.Contains(lastChar))
            {
                words[i] = String.Empty;
            }
        }
        
    }

    return words.Where(w=> w!=String.Empty).ToArray();
}
string IdentifyLanguage(string text)
{
    int ptCount = 0;
    int enCount = 0;
    var words = text.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    foreach (var word in words)
    {
        var cleanedWord = word.Trim().ToLower();
        if (wordsToIgnorePT.Contains(cleanedWord))
        {
            ptCount++;
        }
        if (wordsToIgnoreEN.Contains(cleanedWord))
        {
            enCount++;
        }
    }
    return ptCount > enCount ? "PT" : "EN";
}

public class WordsToClassify
{
    [JsonPropertyName("word")]
    public string Word { get; set; }
    [JsonPropertyName("language")]
    public string Language { get; set; }
    [JsonPropertyName("compound")]
    public bool Compound { get; set; }

    [JsonPropertyName("ignorelist")] 
    public bool IgnoreList { get; set; }
}

public class JobData
{
    [JsonPropertyName("items")]
    public List<JobItem> JobFromJson { get; set; }
}

public class JobItem
{
    [JsonPropertyName("urlJob")]
    public string UrlJob { get; set; }

    [JsonPropertyName("company")]
    public string Company { get; set; }

    [JsonPropertyName("jobTitle")]
    public string JobTitle { get; set; }

    [JsonPropertyName("jobLocal")]
    public string JobLocal { get; set; }

    [JsonPropertyName("jobWhen")]
    public string JobWhen { get; set; }

    [JsonPropertyName("jobCandidatesGross")]
    public string JobCandidatesGross { get; set; }

    [JsonPropertyName("jobDescription")]
    public string JobDescription { get; set; }

    public void PrintShortInfo()
    {
        Console.WriteLine($"Company: {Company}, Job Title: {JobTitle}");
    }
    public void PrintJobInfo()
    {
        Console.WriteLine($"URL: {UrlJob}");
        Console.WriteLine($"Company: {Company}");
        Console.WriteLine($"Job Title: {JobTitle}");
        Console.WriteLine($"Location: {JobLocal}");
        Console.WriteLine($"When: {JobWhen}");
        Console.WriteLine($"Candidates Gross: {JobCandidatesGross}");
        Console.WriteLine($"Description: {JobDescription}");
    }
}


