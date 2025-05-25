// using System;
// using System.IO;
// using UnityEngine;
//
// namespace _StoryGame.Core.Uids
// {
//     public static unsafe class UidGenerator
//     {
//         private const string FILE_NAME = "uid";
//
//         private static string _uidFilePath;
//         private static byte[] _buffer;
//         private static uint _current;
//         private static uint _maxLoaded;
//
//         private static bool _hasChanges;
//
//
//         public static void Init()
//         {
//             _uidFilePath = Path.Combine(Application.persistentDataPath, FILE_NAME);
//
//             if (File.Exists(_uidFilePath))
//             {
//                 _buffer = File.ReadAllBytes(_uidFilePath);
//                 if (_buffer.Length != sizeof(uint))
//                 {
//                     Debug.LogError($"[{nameof(UidGenerator)}] Uid data corrupted");
//                     CreateNew();
//                 }
//             }
//             else
//             {
//                 CreateNew();
//             }
//
//             fixed (byte* ptr = &_buffer[0])
//                 _current = *(uint*)ptr;
//         }
//
//         private static void CreateNew()
//         {
//             _buffer = new byte[sizeof(uint)];
//             File.WriteAllBytes(_uidFilePath, _buffer);
//         }
//
//         public static Uid Next()
//         {
//             if (_current == uint.MaxValue)
//                 throw new Exception($"[{nameof(UidGenerator)}] Uid reached max value: {uint.MaxValue}");
//
//             WriteChanges();
//             return (Uid)(++_current);
//         }
//
//         //Это понадобится когда будет сохранение
//         public static void UpdateMaxLoadedUid(Uid uid)
//         {
//             var loaded = (uint)uid;
//             if (_maxLoaded > loaded)
//                 return;
//
//             _maxLoaded = loaded;
//         }
//
//         //Это понадобится когда будет сохранение
//         public static void RewriteUidIfLoadedGreater()
//         {
//             if (_current >= _maxLoaded)
//                 return;
//
//             WriteChanges();
//             _current = _maxLoaded;
//         }
//
//         private static void WriteChanges()
//         {
//             fixed (byte* ptr = &_buffer[0])
//             {
//                 *(uint*)ptr = _current;
//             }
//
//             File.WriteAllBytes(_uidFilePath, _buffer);
//         }
//     }
// }