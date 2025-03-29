using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Again.Runtime.Commands;
using Again.Runtime.Commands.Camera;
using Again.Runtime.Commands.Dialogue;
using Again.Runtime.Commands.Image;
using Again.Runtime.Commands.OptionMenu;
using Again.Runtime.Commands.Spine;
using Again.Runtime.Commands.Transfer;
using Again.Runtime.Enums;
using UnityEngine;

namespace Again.Runtime.ScriptImpoter
{
    public static class ScriptSheetReader
    {
        private static readonly Stack<OptionMenuCommand> OptionMenuCommandStack = new();
        private static List<Command> commands;
        private static int _currentCommandIndex = -1;

        private static readonly Dictionary<
            string,
            Func<Dictionary<string, string>, Command>
        > CommandCreators =
            new()
            {
                { "LookAtSpine", CreateLookAtSpineCommand },
                { "MoveBackCamera", CreateMoveBackCameraCommand },
                { "ShakeCamera", CreateShakeCameraCommand },
                { "Say", CreateSayCommand },
                { "ShakeDialogue", CreateShakeDialogueCommand },
                { "HideDialogue", CreateHideDialogueCommand },
                { "ChangeSpineColor", CreateChangeSpineColorCommand },
                { "ChangeSpine", CreateChangeSpineCommand },
                { "ChangeSpines", CreateChangeSpinesCommand },
                { "HideSpine", CreateHideSpineCommand },
                { "JumpSpine", CreateJumpSpineCommand },
                { "MoveSpine", CreateMoveSpineCommand },
                { "ScaleSpine", CreateScaleSpineCommand },
                { "ShakeSpine", CreateShakeSpineCommand },
                { "ShowSpine", CreateShowSpineCommand },
                { "Wait", CreateWaitCommand },
                { "ChangeBackground", CreateChangeBackgroundCommand },
                { "HideBackground", CreateHideBackgroundCommand},
                { "ShowTransfer", CreateShowTransferCommand },
                { "HideTransfer", CreateHideTransferCommand },
                { "ChangeImageColor", CreateChangeImageColorCommand },
                { "ChangeImage", CreateChangeImageCommand },
                { "HideImage", CreateHideImageCommand },
                { "JumpImage", CreateJumpImageCommand },
                { "MoveImage", CreateMoveImageCommand },
                { "ScaleImage", CreateScaleImageCommand },
                { "ShakeImage", CreateShakeImageCommand },
                { "ShowImage", CreateShowImageCommand },
                { "OptionMenu", CreateOptionMenuCommand },
                { "OptionMenuEnd", CreateOptionMenuEndCommand },
                { "Option", CreateOptionCommand },
                { "LookAtImage", CreateLookAtImageCommand },
                { "Emit", CreateEmitCommand },
                { "Call", CreateCallCommand },
                { "PlaySound", CreatePlaySoundCommand }
            };


        public static List<Command> Read(List<List<string>> data)
        {
            commands = new List<Command>();
            OptionMenuCommandStack.Clear();
            for (var rowIndex = 0; rowIndex < data.Count; rowIndex++)
            {
                var values = data[rowIndex];
                if (values.Count < 3) continue;
                _currentCommandIndex = rowIndex + 2;

                var commandString = values[0];
                var parameterDict = new Dictionary<string, string>();
                for (var i = 3; i < values.Count; i++)
                    if (values[i].Contains("="))
                    {
                        var parameter = values[i].Split("=");
                        parameterDict.Add(parameter[0], parameter[1]);
                    }

                parameterDict.Add("Command", commandString);
                parameterDict.Add("Character", values[1]);
                parameterDict.Add("Text", values[2]);


                if (CommandCreators.TryGetValue(commandString, out var creator))
                {
                    var command = creator(parameterDict);
                    command.IsJoin = parameterDict.ContainsKey("Join");
                    if (command != null)
                    {
                        command.Id = _currentCommandIndex;
                        commands.Add(command);
                    }
                }
                else
                {
                    Debug.Log($"Line {rowIndex} 找不到指令: {_currentCommandIndex}");
                }
            }

            return commands;
        }

        private static Command CreatePlaySoundCommand(Dictionary<string, string> arg)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "SoundName", Type = "string", CanBeEmpty = false }
            };
            var command = new PlaySoundCommand();
            SetProperties(command, propertyInfos, arg);
            return command;
        }


        private static Command CreateLookAtImageCommand(Dictionary<string, string> arg)
        {
            var properties = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "Scale", Type = "float", CanBeEmpty = true },
                new() { Name = "AnchorX", Type = "float", CanBeEmpty = true },
                new() { Name = "AnchorY", Type = "float", CanBeEmpty = true }
            };
            var command = new LookAtImageCommand();
            SetProperties(command, properties, arg);
            return command;
        }

        private static Command CreateOptionCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Key", Type = "string", CanBeEmpty = true },
                new() { Name = "Text", Type = "string", CanBeEmpty = false }
            };
            var command = new OptionCommand();
            OptionMenuCommandStack.Peek().Options.Add(command);
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static Command CreateOptionMenuEndCommand(Dictionary<string, string> arg)
        {
            var command = new OptionMenuEndCommand();
            if (OptionMenuCommandStack.Count > 0)
            {
                var optionMenuCommand = OptionMenuCommandStack.Pop();
                if (optionMenuCommand.Options.Count == 0)
                    Debug.LogError("OptionMenuEndCommand: 沒有新增選項");

                foreach (var option in optionMenuCommand.Options)
                {
                    option.EndCommand = command;

                    var index = commands.IndexOf(option);
                    if (index == commands.Count - 1)
                    {
                        option.NextCommand = command;
                        continue;
                    }

                    if (commands[index + 1] is OptionCommand)
                        option.NextCommand = command;
                    else
                        option.NextCommand = commands[index + 1];
                }
            }

            return command;
        }

        private static Command CreateOptionMenuCommand(Dictionary<string, string> arg)
        {
            var command = new OptionMenuCommand();
            OptionMenuCommandStack.Push(command);
            return command;
        }

        private static Command CreateShowImageCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "ImageName", Type = "string", CanBeEmpty = true },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "PosX", Type = "float", CanBeEmpty = true },
                new() { Name = "PosY", Type = "float", CanBeEmpty = true },
                new() { Name = "ShowType", Type = "ShowAnimationType", CanBeEmpty = true },
                new() { Name = "NextDuration", Type = "float", CanBeEmpty = true },
                new() { Name = "Order", Type = "int", CanBeEmpty = true }
            };

            var command = new ShowImageCommand();
            SetProperties(command, propertyInfos, dict);
            SetScaleProperty(command, dict);
            
            // 沒有指定 ImageName 就用 Name
            if (!dict.ContainsKey("ImageName"))
                command.ImageName = command.Name;
            
            return command;
        }

        private static Command CreateShakeImageCommand(Dictionary<string, string> arg)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "Strength", Type = "float", CanBeEmpty = true },
                new() { Name = "Vibrato", Type = "int", CanBeEmpty = true },
                new() { Name = "Randomness", Type = "float", CanBeEmpty = true },
                new() { Name = "Snapping", Type = "boolean", CanBeEmpty = true },
                new() { Name = "FadeOut", Type = "boolean", CanBeEmpty = true },
                new() { Name = "ShakeType", Type = "ShakeType", CanBeEmpty = true }
            };
            var command = new ShakeImageCommand();
            SetProperties(command, propertyInfos, arg);
            return command;
        }


        private static Command CreateScaleImageCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "AnchorX", Type = "float", CanBeEmpty = true },
                new() { Name = "AnchorY", Type = "float", CanBeEmpty = true }
            };
            var command = new ScaleImageCommand();
            SetProperties(command, propertyInfos, dict);
            SetScaleProperty(command, dict);
            return command;
        }

        private static Command CreateMoveImageCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "PosX", Type = "float", CanBeEmpty = true },
                new() { Name = "PosY", Type = "float", CanBeEmpty = true }
            };
            var command = new MoveImageCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static Command CreateJumpImageCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "JumpPower", Type = "float", CanBeEmpty = true },
                new() { Name = "JumpCount", Type = "int", CanBeEmpty = true }
            };

            var command = new JumpImageCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static Command CreateHideImageCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "HideType", Type = "HideAnimationType", CanBeEmpty = true }
            };
            var command = new HideImageCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }


        private static Command CreateChangeImageCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "ImageName", Type = "string", CanBeEmpty = false }
            };
            var command = new ChangeImageCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static Command CreateChangeImageColorCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "ChangeColorType", Type = "ChangeColorType", CanBeEmpty = true }
            };
            var command = new ChangeImageColorCommand();
            SetProperties(command, propertyInfos, dict);
            if (dict.TryGetValue("Color", out var colorString))
                command.ColorDelta = ParseColorString(colorString);
            return command;
        }

        private static Command CreateHideTransferCommand(Dictionary<string, string> dict)
        {
            return new HideTransferCommand();
        }

        private static Command CreateShowTransferCommand(Dictionary<string, string> dict)
        {
            return new ShowTransferCommand();
        }

        private static Command CreateChangeBackgroundCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "ImageName", Type = "string", CanBeEmpty = true },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name =  "ShowType", Type = "ShowAnimationType", CanBeEmpty = true },
            };
            var command = new ChangeBackgroundCommand();
            SetProperties(command, propertyInfos, dict);
            dict.TryGetValue("Color", out var colorString);
            command.Color = ParseColorString(colorString);
            return command;
        }

        private static Command CreateHideBackgroundCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
            };
            var command = new HideBackgroundCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static WaitCommand CreateWaitCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Duration", Type = "float", CanBeEmpty = true }
            };
            var command = new WaitCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static ShowSpineCommand CreateShowSpineCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "SpineName", Type = "string", CanBeEmpty = true },
                new() { Name = "Animation", Type = "string", CanBeEmpty = false },
                new() { Name = "Skin", Type = "string", CanBeEmpty = true },
                new() { Name = "PosX", Type = "float", CanBeEmpty = true },
                new() { Name = "PosY", Type = "float", CanBeEmpty = true },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "ShowType", Type = "ShowAnimationType", CanBeEmpty = true },
                new() { Name = "IsLoop", Type = "boolean", CanBeEmpty = true },
                new() { Name = "Order", Type = "int", CanBeEmpty = true }
            };

            var showSpineCommand = new ShowSpineCommand();
            SetProperties(showSpineCommand, propertyInfos, dict);
            SetScaleProperty(showSpineCommand, dict);
            
            // 沒有指定 SpineName 就用 Name
            if (!dict.ContainsKey("SpineName"))
                showSpineCommand.SpineName = showSpineCommand.Name;
            return showSpineCommand;
        }

        private static SayCommand CreateSayCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Character", Type = "string", CanBeEmpty = true },
                new() { Name = "Text", Type = "string", CanBeEmpty = false },
                new() { Name = "Voice", Type = "string", CanBeEmpty = true },
                new() { Name = "Scale", Type = "float", CanBeEmpty = true },
                new() { Name = "Key", Type = "string", CanBeEmpty = true }
            };
            var command = new SayCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static ShakeSpineCommand CreateShakeSpineCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "Strength", Type = "float", CanBeEmpty = true },
                new() { Name = "Vibrato", Type = "int", CanBeEmpty = true },
                new() { Name = "Randomness", Type = "float", CanBeEmpty = true },
                new() { Name = "Snapping", Type = "boolean", CanBeEmpty = true },
                new() { Name = "ShakeType", Type = "ShakeType", CanBeEmpty = true }
            };

            var command = new ShakeSpineCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static ScaleSpineCommand CreateScaleSpineCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "AnchorX", Type = "float", CanBeEmpty = true },
                new() { Name = "AnchorY", Type = "float", CanBeEmpty = true }
            };

            var command = new ScaleSpineCommand();
            SetProperties(command, propertyInfos, dict);
            SetScaleProperty(command, dict);
            return command;
        }

        private static MoveSpineCommand CreateMoveSpineCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "PosX", Type = "float", CanBeEmpty = true },
                new() { Name = "PosY", Type = "float", CanBeEmpty = true }
            };

            var command = new MoveSpineCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static JumpSpineCommand CreateJumpSpineCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "JumpPower", Type = "float", CanBeEmpty = true },
                new() { Name = "JumpCount", Type = "int", CanBeEmpty = true }
            };

            var command = new JumpSpineCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static HideSpineCommand CreateHideSpineCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "HideType", Type = "HideAnimationType", CanBeEmpty = true }
            };

            var command = new HideSpineCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static ChangeSpineCommand CreateChangeSpineCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Skin", Type = "string", CanBeEmpty = true },
                new() { Name = "Animation", Type = "string", CanBeEmpty = true },
                new() { Name = "IsLoop", Type = "boolean", CanBeEmpty = true }
            };

            var command = new ChangeSpineCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static ChangeSpinesCommand CreateChangeSpinesCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Skin", Type = "string", CanBeEmpty = true },
                new() { Name = "IsLoop", Type = "boolean", CanBeEmpty = true }
            };
            dict.TryGetValue("Animations", out var animsStr);

            var animations = new List<string>();
            if (!string.IsNullOrEmpty(animsStr)) animations.AddRange(animsStr.Split(','));

            var command = new ChangeSpinesCommand();
            command.Animations = animations;
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static ChangeSpineColorCommand CreateChangeSpineColorCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "ChangeColorType", Type = "ChangeColorType", CanBeEmpty = true }
            };

            var command = new ChangeSpineColorCommand();
            if (dict.TryGetValue("Color", out var colorString))
                command.ColorDelta = ParseColorString(colorString);
            SetProperties(command, propertyInfos, dict);
            return command;
        }


        private static ShakeDialogueCommand CreateShakeDialogueCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Duration", Type = "float", CanBeEmpty = true }
            };
            var command = new ShakeDialogueCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static Command CreateHideDialogueCommand(Dictionary<string, string> arg)
        {
            var command = new HideDialogueCommand();
            return command;
        }

        private static ShakeCameraCommand CreateShakeCameraCommand(
            Dictionary<string, string> dict
        )
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Duration", Type = "float", CanBeEmpty = true }
            };
            var command = new ShakeCameraCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static LookAtSpineCommand CreateLookAtSpineCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false },
                new() { Name = "Duration", Type = "float", CanBeEmpty = true },
                new() { Name = "Scale", Type = "float", CanBeEmpty = true },
                new() { Name = "AnchorX", Type = "float", CanBeEmpty = true },
                new() { Name = "AnchorY", Type = "float", CanBeEmpty = true }
            };

            var command = new LookAtSpineCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static Command CreateMoveBackCameraCommand(Dictionary<string, string> dict)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Duration", Type = "float", CanBeEmpty = true }
            };
            var command = new MoveBackCameraCommand();
            SetProperties(command, propertyInfos, dict);
            return command;
        }

        private static Command CreateEmitCommand(Dictionary<string, string> arg)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "Name", Type = "string", CanBeEmpty = false }
            };

            var command = new EmitCommand();
            SetProperties(command, propertyInfos, arg);
            var parameters = new List<string>();

            var i = 1;
            while (arg.TryGetValue($"Param{i}", out var param))
            {
                parameters.Add(param);
                i++;
            }

            command.Parameters = parameters;
            return command;
        }

        private static Command CreateCallCommand(Dictionary<string, string> arg)
        {
            var propertyInfos = new List<PropertyInfo>
            {
                new() { Name = "ScriptName", Type = "string", CanBeEmpty = false }
            };

            var command = new CallCommand();
            SetProperties(command, propertyInfos, arg);
            return command;
        }

        private static Color ParseColorString(string color)
        {
            if (string.IsNullOrEmpty(color)) return Color.white;
            var matches = Regex.Matches(color, @"\d+");
            var c = new Color();
            if (matches.Count == 3)
            {
                c.r = float.Parse(matches[0].Value) / 255;
                c.g = float.Parse(matches[1].Value) / 255;
                c.b = float.Parse(matches[2].Value) / 255;
                c.a = 1;
            } else if (matches.Count == 4)
            {
                c.r = float.Parse(matches[0].Value) / 255;
                c.g = float.Parse(matches[1].Value) / 255;
                c.b = float.Parse(matches[2].Value) / 255;
                c.a = float.Parse(matches[3].Value) / 255;
            }
            return c;
        }

        private static void SetProperties(Command command, List<PropertyInfo> propertyInfos,
            Dictionary<string, string> dict)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                dict.TryGetValue(propertyInfo.Name, out var stringValue);

                if (string.IsNullOrEmpty(stringValue))
                {
                    if (propertyInfo.CanBeEmpty)
                        continue;

                    Debug.LogError(
                        $"Line {_currentCommandIndex} {command.GetType().Name.Replace("Command", "")} {propertyInfo.Name} 必填");
                    continue;
                }

                try
                {
                    switch (propertyInfo.Type)
                    {
                        case "string":
                            command.GetType().GetProperty(propertyInfo.Name)?.SetValue(command, stringValue);
                            break;
                        case "int":
                            command.GetType().GetProperty(propertyInfo.Name)?.SetValue(command, int.Parse(stringValue));
                            break;
                        case "float":
                            var value = float.Parse(stringValue);
                            command.GetType().GetProperty(propertyInfo.Name)?.SetValue(command, value);
                            break;
                        case "boolean":
                            command.GetType().GetProperty(propertyInfo.Name)
                                ?.SetValue(command, bool.Parse(stringValue));
                            break;
                        case "HideAnimationType":
                            command.GetType().GetProperty(propertyInfo.Name)?.SetValue(
                                command,
                                Enum.Parse(typeof(HideAnimationType), stringValue)
                            );
                            break;
                        case "ChangeColorType":
                            command.GetType().GetProperty(propertyInfo.Name)?.SetValue(
                                command,
                                Enum.Parse(typeof(ChangeColorType), stringValue)
                            );
                            break;
                        case "ShakeType":
                            command.GetType().GetProperty(propertyInfo.Name)?.SetValue(
                                command,
                                Enum.Parse(typeof(ShakeType), stringValue)
                            );
                            break;
                        case "ShowAnimationType":
                            command.GetType().GetProperty(propertyInfo.Name)?.SetValue(
                                command,
                                Enum.Parse(typeof(ShowAnimationType), stringValue)
                            );
                            break;
                        default:
                            Debug.LogError($"Type {propertyInfo.Type} not supported");
                            break;
                    }
                }
                catch (FormatException)
                {
                    Debug.LogError(
                        $"Line {_currentCommandIndex} {command.GetType().Name.Replace("Command", "")} {propertyInfo.Name} {stringValue} 格式錯誤");
                }
            }
        }
        
        private static void SetScaleProperty(IScalableCommand command, Dictionary<string, string> dict)
        {
            if (dict.TryGetValue("Scale", out var scaleString))
            {
                command.ScaleX = command.ScaleY = float.Parse(scaleString);
                return;
            }
            
            if (dict.TryGetValue("ScaleX", out var scaleX))
                command.ScaleX = float.Parse(scaleX);
            if (dict.TryGetValue("ScaleY", out var scaleY))
                command.ScaleY = float.Parse(scaleY);
        }
    }
}