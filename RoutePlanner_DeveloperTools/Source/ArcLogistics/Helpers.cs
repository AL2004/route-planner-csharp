﻿/*
 | Version 10.1.84
 | Copyright 2013 Esri
 |
 | Licensed under the Apache License, Version 2.0 (the "License");
 | you may not use this file except in compliance with the License.
 | You may obtain a copy of the License at
 |
 |    http://www.apache.org/licenses/LICENSE-2.0
 |
 | Unless required by applicable law or agreed to in writing, software
 | distributed under the License is distributed on an "AS IS" BASIS,
 | WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 | See the License for the specific language governing permissions and
 | limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ESRI.ArcLogistics
{
    static class FileHelpers
    {
        public static bool IsAbsolutPath(string path)
        {
            bool result = false;

            if (char.IsLetter(path[0]) && path[1] == ':' && IsSlash(path[2]))
            {
                result = true;
            }

            return result;
        }
        
        public static bool IsSlash(char letter)
        {
            if (letter == '/' || letter == '\\')
                return true;
            else
                return false;
        }


        public static string FixSlash(string path, bool needSlash)
        {
            if (needSlash)
            {
                if (path.Length > 0 && !IsSlash(path[path.Length - 1]))
                {
                    path = path + "\\";
                }
            }
            else
            {
                if (path.Length > 0 && IsSlash(path[path.Length - 1]))
                    path.Remove(path.Length - 1, 1);
            }

            return path;
        }

        static public bool IsFileNameCorrect(string fileName)
        {
            bool bOk = false;
            try
            {
                new System.IO.FileInfo(fileName);
                bOk = true;
            }
            catch (ArgumentException)
            {
            }
            catch (System.IO.PathTooLongException)
            {
            }
            catch (NotSupportedException)
            {
            }
            return bOk;
        }
            
        /// <summary>
        /// Gets whether the specified path is a valid absolute file path.
        /// </summary>
        /// <param name="path">Any path. OK if null or empty.</param>
        public static bool ValidateFilepath(string path)
        {
            if (path.Trim() == string.Empty)
            {
                return false;
            }

            string pathName = string.Empty;
            string fileName = string.Empty;

            bool result = true;
            try
            {
                fileName = System.IO.Path.GetFileName(path);
                pathName = System.IO.Path.GetPathRoot(path);
            }
            catch
            {
                result = false;
            }

            if (!result)
                return false;
            else
            {
                if (fileName.Trim() == string.Empty)
                {
                    return false;
                }

                if (fileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
                {
                    return false;
                }

                if (pathName.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
                {
                    return false;
                }
            }
            return true;
        }

        //public static bool ValidateFilePath(string filePath)
        //{
        //    Regex r = new Regex( @"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$" );
        //    return r.IsMatch(filePath);
        //}

        static public bool CheckWriteAccess(string path)
        {
            bool result = true;

            FileSecurity security = File.GetAccessControl(path,
      AccessControlSections.Access | AccessControlSections.Owner);
             NTAccount owner = (NTAccount)security.GetOwner(typeof(NTAccount));

            Console.WriteLine("Owner: {0}", owner.Value);

            // DACLs
            foreach (FileSystemAccessRule rule in security.GetAccessRules(true, true,
              typeof(NTAccount)))
            {
                Console.WriteLine("{0} {1} access to {2}",
                  rule.AccessControlType == AccessControlType.Allow ?
                  "grant: " : "deny: ", rule.FileSystemRights,
                  rule.IdentityReference.ToString());
            }
            return result;
        }
    }
}
