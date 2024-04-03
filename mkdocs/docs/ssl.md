# SSL

SSL is managed internally by creating a local certificate authority key on your development machine, adding that root certificate as trusted by your operating system and web browser, then generating a wildcard key for a seasprig domain that traefik can then use to serve websites with a key your development computer trusts.

In order for this to work, we must first install [mkcert](https://github.com/FiloSottile/mkcert) which allows us to add a local trusted authority that can craft keys.

Once mkcert is installed, it will come with a trusted certificate key. This needs to be enabled with the install command:

```
mkcert -install
```

Once this is installed, your operating system will trust SSL certificates signed by your local mkcert install. One catch is that firefox on windows doesn't use the operating system for its trusted authorities, and you may have to go into the firefox settings and add the key there, too.

You can find where mkcert holds the local key it has installed by running the following:

```
mkcert -CAROOT
```

Going to the firefox security settings and selecting the key at this location to trust should complete the installation of the trusted root certificate.

Now, it is time to generate a certificate for our website. Traefik looks for certificates in the ./traefik directory, in the same directory as the Dockerfile. Navigate to this directory, and generate a wildcard key for *.seasprig.dev using the following command:

```
mkcert -key-file seasprig.key -cert-file seasprig.cert seasprig.dev *.seasprig.dev localhost 127.0.0.1
```

Its worth noting that any domains traefik will route will also need to be pointed to traefik via DNS.