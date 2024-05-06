# Installation

To install this system and get up and running, there are a few prerequisites you will need as well as several recommended tools.



migrating to microk8s (windows):

* For windows install, you need an up-to-date system
* wsl2 installed and up to date, with an up-to-date ubuntu
* snapd must be installed, and this means you need systemd enabled: https://microk8s.io/docs/install-wsl2

migrating to microk8s (windows and linux):

* follow tilt guide for microk8s here: https://docs.tilt.dev/choosing_clusters.html#microk8s

```
sudo snap install microk8s --classic && \
sudo microk8s.enable dns && \
sudo microk8s.enable registry

```

```
sudo snap install kubectl --classic
sudo snap install helm --classic
sudo snap install k9s
sudo ln -s /snap/k9s/current/bin/k9s /snap/bin/
mkdir ~/.kube
```

install docker
https://docs.docker.com/engine/install/ubuntu/#install-using-the-repository

```
# Add Docker's official GPG key:
sudo apt-get update
sudo apt-get install ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

# Add the repository to Apt sources:
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update

sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```


post-docker install:
```
sudo groupadd docker
sudo usermod -aG docker $USER
```
then log out and back in again


```
sudo microk8s.kubectl config view --flatten > ~/.kube/microk8s-config && \
KUBECONFIG=~/.kube/microk8s-config:~/.kube/config kubectl config view --flatten > ~/.kube/temp-config && \
mv ~/.kube/temp-config ~/.kube/config && \
kubectl config use-context microk8s

```

to export the config to another machine (or from linux to the windows host)
```
microk8s config > /tmp/config
```
then move the output file where you need it

necessary to run:

* [tilt](https://docs.tilt.dev/install.html) (to launch the application stack)

recommended for development:

If you want to open the browser on windows when launching tilt in wsl2:

```
sudo add-apt-repository ppa:wslutilities/wslu
sudo apt update
sudo apt install wslu
```

```
sudo apt-get install -y dotnet-sdk-8.0
```

install homebrew and mirrord cli:
```
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
(echo; echo 'eval "$(/home/linuxbrew/.linuxbrew/bin/brew shellenv)"') >> /home/kyle/.bashrc
eval "$(/home/linuxbrew/.linuxbrew/bin/brew shellenv)"
sudo apt-get install build-essential
brew install gcc
brew install metalbear-co/mirrord/mirrord
```

* visual studio code
* nvm (to run typescript applications locally)
* pgadmin (to explore the local database)
