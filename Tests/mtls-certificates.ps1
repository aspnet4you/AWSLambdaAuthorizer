# Change the folder
cd C:\Users\cloudninja\aws\mtls

# OpenSSL is available under Git and it is trusted. Use it from there.

&"C:\Program Files\Git\usr\bin\openssl.exe" genrsa -out RootCA.key 4096
&"C:\Program Files\Git\usr\bin\openssl.exe" req -new -x509 -days 3650 -key RootCA.key -out RootCA.pem
&"C:\Program Files\Git\usr\bin\openssl.exe" genrsa -out my_client.key 2048
&"C:\Program Files\Git\usr\bin\openssl.exe" req -new -key my_client.key -out my_client.csr
&"C:\Program Files\Git\usr\bin\openssl.exe" x509 -req -in my_client.csr -CA RootCA.pem -CAkey RootCA.key -set_serial 01 -out my_client.pem -days 3650 -sha256
&"C:\Program Files\Git\usr\bin\openssl.exe" pkcs12 -inkey my_client.key -in my_client.pem -export -out my_client.pem.pfx
&"C:\Program Files\Git\usr\bin\openssl.exe" pkcs12 -inkey RootCA.key -in RootCA.pem -export -out RootCA.pem.pfx

cp RootCA.pem truststore.pem

$bucketName = 'aspnet4you-ca-truststore'
aws s3 mb s3://$($bucketName) --region=us-east-1
aws s3api put-bucket-versioning --bucket $($bucketName) --versioning-configuration Status=Enabled
aws s3 cp truststore.pem s3://$($bucketName)/truststore.pem

# Positive tests
# Windows machine : 
# Import my_client.pem.pfx into current user's personal certificate folder. No password for this demo cert.
# Import RootCA.pem.pfx into current user's trusted certificate authority folder. No password for this demo cert.
Invoke-WebRequest https://mtlsapi.aspnet4you.com/pets -CertificateThumbprint 3063d99f0a3ea1c6272eee17aeb5a2db9ab89a00
Invoke-RestMethod https://mtlsapi.aspnet4you.com/pets -CertificateThumbprint 3063d99f0a3ea1c6272eee17aeb5a2db9ab89a00
Invoke-RestMethod https://mtlsapi.aspnet4you.com/pets/3 -CertificateThumbprint 3063d99f0a3ea1c6272eee17aeb5a2db9ab89a00 

# Create another client certificate singed by same root CA for negative testing
&"C:\Program Files\Git\usr\bin\openssl.exe" genrsa -out my_client2.key 2048
&"C:\Program Files\Git\usr\bin\openssl.exe" req -new -key my_client2.key -out my_client2.csr
&"C:\Program Files\Git\usr\bin\openssl.exe" x509 -req -in my_client2.csr -CA RootCA.pem -CAkey RootCA.key -set_serial 01 -out my_client2.pem -days 3650 -sha256
&"C:\Program Files\Git\usr\bin\openssl.exe" pkcs12 -inkey my_client2.key -in my_client2.pem -export -out my_client2.pem.pfx

# Negative test
# Windows machine : 
# Import my_client2.pem.pfx into current user's personal certificate folder. No password for this demo cert.
Invoke-RestMethod https://mtlsapi.aspnet4you.com/pets -CertificateThumbprint 57f7ab0f2935479f98aedad3ec459ca4187acecc