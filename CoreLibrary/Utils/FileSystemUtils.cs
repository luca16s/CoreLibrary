// <copyright file="FileSystemUtils.cs" company="Îakaré Software'oka">
//     Copyright (c) Îakaré Software'oka. All rights reserved. Licensed under the MIT license. See
//     LICENSE file in the project root for full license information.
// </copyright>

namespace CoreLibrary.Utils
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Encapsula operações comuns do sistema de arquivos Windows.
    /// </summary>
    public static class FileSystemUtils
    {
        private static int MAX_DELETE_ATTEMPTS;
        private const string PlataformNotSupportedMessage = "Somente windows é suportado nesta operação.";

        /// <summary>
        /// Apaga determinado arquivo baseado em um caminho específico.
        /// </summary>
        /// <param name="path">
        /// Endereço no sistema de arquivos.
        /// </param>
        /// <returns>
        /// Se arquivo ou pasta foi apagado com sucesso.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// Sistema operacional não suportado.
        /// </exception>
        public static bool DeleteFile(string path)
        {
            MAX_DELETE_ATTEMPTS++;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException(PlataformNotSupportedMessage);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
                MAX_DELETE_ATTEMPTS = 0;
            }

            return !File.Exists(path)
                || DeleteFile(path)
                || MAX_DELETE_ATTEMPTS == 10;
        }

        /// <summary>
        /// Encontra o endereço da pasta no sistema de arquivos.
        /// </summary>
        /// <param name="folder">
        /// Nome da pasta a ser encontrada.
        /// </param>
        /// <returns>
        /// Retorna o endereço encontrado.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// Sistema operacional não suportado.
        /// </exception>
        public static string FindFolderPath(string folder)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException(PlataformNotSupportedMessage);
            }

            Environment.SpecialFolder[] specialFolders = (Environment.SpecialFolder[])Enum.GetValues(typeof(Environment.SpecialFolder));
            for (int i = 0; i < specialFolders.Length; i++)
            {
                string? folderLocation = GetEspecialFolderPath(specialFolders[i]);
                string? pathCombination = Path.Combine(folderLocation, folder);

                if (Directory.Exists(pathCombination))
                {
                    return pathCombination;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Retorna o caminho de uma pasta especial.
        /// </summary>
        /// <returns>
        /// Caminho para pasta.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// Sistema operacional não suportado.
        /// </exception>
        public static string GetEspecialFolderPath(Environment.SpecialFolder specialFolder)
        {
            return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? throw new PlatformNotSupportedException(PlataformNotSupportedMessage)
                : Environment.GetFolderPath(specialFolder);
        }
    }
}