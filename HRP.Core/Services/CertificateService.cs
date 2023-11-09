namespace HRP.Core.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using HRP.Core.Constants;
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Caching.Memory;
    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Asn1.Pkcs;
    using Org.BouncyCastle.Asn1.X509;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Operators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Crypto.Prng;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.OpenSsl;
    using Org.BouncyCastle.Pkcs;
    using Org.BouncyCastle.Security;
    using Org.BouncyCastle.Utilities;
    using Org.BouncyCastle.X509;

    public class CertificateService : ICertificateService
    {
        private const string SubjectName = "CN=HRP";
        private const string IssuerName = "CN=HRP";
        private const int KeyStrength = 2048;
        private const int ValidSequenceCount = 9;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IMemoryCache memoryCache;
        private readonly IBaseService baseService;

        public CertificateService(
            IHostingEnvironment hostingEnvironment,
            IMemoryCache memoryCache,
            IBaseService baseService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.memoryCache = memoryCache;
            this.baseService = baseService;
        }

        public string GetCertificateFilePath()
        {
            string filePath = Path.Combine(hostingEnvironment.ContentRootPath, Constants.CertificateFilePath, Constants.CertificateFileName);
            return filePath;
        }

        public string GetPasswordAccessTheCertificate()
        {
            memoryCache.TryGetValue(CacheKeys.PasswordAccessTheCertificate, out string password);

            return password;
        }

        public CommandResult GenerateSelfSignedCertificate(string password)
        {
            var commandResult = new CommandResult
            {
                Code = 0
            };

            X509Certificate2 cert = GenerateSelfSignedCertificate();
            byte[] certData = cert.Export(X509ContentType.Pfx, password);
            string filePath = GetCertificateFilePath();
            File.WriteAllBytes(filePath, certData);

            return commandResult;
        }

        public bool IsGeneratedCertificate()
        {
            var criteria = new GetRefResourceByNamesCriteria
            {
                RefNames = ReferenceResource.IsGeneratedCertificate,
                IsActive = true
            };
            var refResource = baseService.GetDataEntity<ReferenceResourceModel>(criteria);

            return refResource.Value.Equals("1");
        }

        public bool IsExistedCertificate()
        {
            string filePath = GetCertificateFilePath();
            return File.Exists(filePath);
        }

        public bool IsCorrectPasswordAccessTheCertificate(string password)
        {
            X509Certificate2 cert;

            try
            {
                string filePath = GetCertificateFilePath();
                const X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet;
                cert = new X509Certificate2(filePath, password, flags);
            }
            catch
            {
                return false;
            }

            return cert != null;
        }

        public void CachePasswordAccessTheCertificate(string password)
        {
            var cacheExpirationOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(30),
                Priority = CacheItemPriority.Normal
            };
            memoryCache.Set(CacheKeys.PasswordAccessTheCertificate, password, cacheExpirationOptions);
        }

        public CommandResult ChangePasswordAccessTheCertificate(string oldPassword, string password)
        {
            var commandResult = new CommandResult
            {
                Code = 0
            };

            try
            {
                string filePath = GetCertificateFilePath();
                var cert = new X509Certificate2(filePath, oldPassword, X509KeyStorageFlags.Exportable);
                File.WriteAllBytes(filePath, cert.Export(X509ContentType.Pfx, password));
            }
            catch (CryptographicException ex)
            {
                commandResult.Code = 1;
                commandResult.Message = ex.Message;
            }

            return commandResult;
        }

        public byte[] CopyCertificateFileToByteArray()
        {
            string filePath = GetCertificateFilePath();
            string password = GetPasswordAccessTheCertificate();
            var cert = new X509Certificate2(filePath, password, X509KeyStorageFlags.Exportable);
            byte[] certData = cert.Export(X509ContentType.Pfx, password);

            return certData;
        }

        private static X509Certificate2 GenerateSelfSignedCertificate()
        {
            // Generating Random Numbers
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);

            // The Certificate Generator
            var certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            // Issuer and Subject Name
            var subjectDN = new X509Name(SubjectName);
            var issuerDN = new X509Name(IssuerName);
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            var notBefore = DateTimeOffset.UtcNow.Date;
            var notAfter = notBefore.AddYears(50);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // Subject Public Key
            AsymmetricCipherKeyPair subjectKeyPair;
            var keyGenerationParameters = new KeyGenerationParameters(random, KeyStrength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            // Generating the Certificate
            AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;

            ISignatureFactory signatureFactory = new Asn1SignatureFactory(
                PkcsObjectIdentifiers.Sha512WithRsaEncryption.ToString(),
                issuerKeyPair.Private,
                random);

            // selfsign certificate
            var certificate = certificateGenerator.Generate(signatureFactory);

            // correcponding private key
            PrivateKeyInfo info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectKeyPair.Private);

            // merge into X509Certificate2
            var x509 = new X509Certificate2(certificate.GetEncoded());

            var seq = (Asn1Sequence)Asn1Object.FromByteArray(info.ParsePrivateKey().GetDerEncoded());
            if (seq.Count != ValidSequenceCount)
            {
                throw new PemException("malformed sequence in RSA private key");
            }

            var rsa = RsaPrivateKeyStructure.GetInstance(seq);
            var rsaparams = new RsaPrivateCrtKeyParameters(
                rsa.Modulus,
                rsa.PublicExponent,
                rsa.PrivateExponent,
                rsa.Prime1, rsa.Prime2,
                rsa.Exponent1,
                rsa.Exponent2,
                rsa.Coefficient);

            x509.PrivateKey = DotNetUtilities.ToRSA(rsaparams);

            return x509;
        }
    }
}