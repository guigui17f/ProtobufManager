using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Google.Protobuf.Reflection;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace GUIGUI17F.ProtobufManager
{
    public class ProtoCompilerUtility
    {
        private static StringBuilder _builder = new StringBuilder();
        private static StringBuilder _outputBuilder = new StringBuilder();
        private static StringBuilder _errorBuilder = new StringBuilder();
        private static string _compilerPath;
        private static string _descriptionCachePath;

        /// <summary>
        /// generate scripts base on the provided proto files
        /// </summary>
        /// <param name="protoPaths">proto file paths used for generate scripts</param>
        /// <param name="config">config data for the generation</param>
        /// <returns>generation completed with no error</returns>
        public static bool GenerateScripts(List<string> protoPaths, ScriptGenerationConfig config)
        {
            _builder.Clear();
            List<string> importPaths = ProtobufManagerConfigs.GetImportPaths(true);
            foreach (string path in importPaths)
            {
                _builder.Append("--proto_path=\"");
                _builder.Append(path);
                _builder.Append("\" ");
            }
            switch (config.Language)
            {
                case TargetLanguage.Cpp:
                    _builder.Append("--cpp_out=\"");
                    break;
                case TargetLanguage.CSharp:
                    _builder.Append("--csharp_out=\"");
                    break;
                case TargetLanguage.Java:
                    _builder.Append("--java_out=\"");
                    break;
                case TargetLanguage.Kotlin:
                    _builder.Append("--kotlin_out=\"");
                    break;
                case TargetLanguage.Objc:
                    _builder.Append("--objc_out=\"");
                    break;
                case TargetLanguage.PHP:
                    _builder.Append("--php_out=\"");
                    break;
                case TargetLanguage.Python:
                    _builder.Append("--python_out=\"");
                    break;
                case TargetLanguage.Ruby:
                    _builder.Append("--ruby_out=\"");
                    break;
                case TargetLanguage.Dart:
                    _builder.Append("--dart_out\"");
                    break;
                case TargetLanguage.Go:
                    _builder.Append("--go_out=\"");
                    break;
            }
            _builder.Append(GlobalPathUtility.GetRelativePath(config.Path));
            _builder.Append("\" ");
            if (!string.IsNullOrEmpty(config.ExtraParameters))
            {
                _builder.Append(config.ExtraParameters);
                _builder.Append(' ');
            }
            foreach (string path in protoPaths)
            {
                _builder.Append('"');
                _builder.Append(GlobalPathUtility.GetRelativePath(path));
                _builder.Append("\" ");
            }
            string generationCommand = _builder.ToString();
            Debug.Log($"generation command: {generationCommand}");

            if (!Directory.Exists(config.Path))
            {
                Directory.CreateDirectory(config.Path);
            }
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = GetCompilerPath(),
                    WorkingDirectory = GlobalPathUtility.GetProjectPath(),
                    Arguments = generationCommand,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };
            _outputBuilder.Clear();
            _errorBuilder.Clear();
            process.OutputDataReceived += OnOutputDataReceived;
            process.ErrorDataReceived += OnErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Close();

            bool hasError = false;
            if (_outputBuilder.Length > 0)
            {
                Debug.Log(_outputBuilder.ToString());
                _outputBuilder.Clear();
            }
            if (_errorBuilder.Length > 0)
            {
                hasError = true;
                Debug.LogError(_errorBuilder.ToString());
                _errorBuilder.Clear();
            }
            AssetDatabase.Refresh();
            return !hasError;
        }

        /// <summary>
        /// get the description data of a proto file
        /// </summary>
        /// <param name="protoPath">path of the target proto file</param>
        /// <returns>the description data, null if generation failed</returns>
        public static FileDescriptorSet GetFileDescription(string protoPath)
        {
            if (string.IsNullOrEmpty(_descriptionCachePath))
            {
                string pluginDirectory = GlobalPathUtility.GetPluginPath();
                _descriptionCachePath = GlobalPathUtility.CombinePath(pluginDirectory, "Editor/Caches/proto_description_cache.pb");
            }

            _builder.Clear();
            List<string> importPaths = ProtobufManagerConfigs.GetImportPaths(true);
            foreach (string path in importPaths)
            {
                _builder.Append("--proto_path=\"");
                _builder.Append(path);
                _builder.Append("\" ");
            }
            _builder.Append("--descriptor_set_out=\"");
            _builder.Append(_descriptionCachePath);
            _builder.Append("\" ");
            _builder.Append('"');
            _builder.Append(GlobalPathUtility.GetRelativePath(protoPath));
            _builder.Append('"');
            string generationCommand = _builder.ToString();
            //Debug.Log($"parse command: {generationCommand}");

            if (File.Exists(_descriptionCachePath))
            {
                File.Delete(_descriptionCachePath);
            }
            string directory = Path.GetDirectoryName(_descriptionCachePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = GetCompilerPath(),
                    WorkingDirectory = GlobalPathUtility.GetProjectPath(),
                    Arguments = generationCommand,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            _outputBuilder.Clear();
            _errorBuilder.Clear();
            process.OutputDataReceived += OnOutputDataReceived;
            process.ErrorDataReceived += OnErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Close();

            if (_outputBuilder.Length > 0)
            {
                Debug.Log(_outputBuilder.ToString());
                _outputBuilder.Clear();
            }
            if (_errorBuilder.Length > 0)
            {
                Debug.LogError(_errorBuilder.ToString());
                _errorBuilder.Clear();
            }
            FileDescriptorSet fileSet = null;
            if (File.Exists(_descriptionCachePath))
            {
                using (FileStream stream = File.OpenRead(_descriptionCachePath))
                {
                    fileSet = FileDescriptorSet.Parser.ParseFrom(stream);
                }
                File.Delete(_descriptionCachePath);
            }
            return fileSet;
        }

        public static bool IsCompilerExist()
        {
            bool compilerExist = File.Exists(GetCompilerPath());
            if (!compilerExist)
            {
                MessageDialog.DisplayDialog("Warning", "Protobuf compiler doesn't exist, please integrate the compiler following instructions in the user guide document.", "OK");
            }
            return compilerExist;
        }

        private static string GetCompilerPath()
        {
            if (string.IsNullOrEmpty(_compilerPath))
            {
#if UNITY_EDITOR_WIN
                string compilerName = "protoc.exe";
#else
                string compilerName = "protoc";
#endif
                string pluginDirectory = GlobalPathUtility.GetPluginPath();
                string compilerPath = GlobalPathUtility.CombinePath(pluginDirectory, "Editor/Compiler/bin", compilerName);
                _compilerPath = Path.GetFullPath(compilerPath);
            }
            return _compilerPath;
        }

        private static void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _outputBuilder.AppendLine(e.Data);
            }
        }

        private static void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _errorBuilder.AppendLine(e.Data);
            }
        }
    }
}