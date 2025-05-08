sudo apt-get update && sudo apt-get install -y dotnet-sdk-9.0
sudo apt-get update && sudo apt-get install -y dotnet-runtime-9.0

sudo apt-get update && sudo apt-get install -y git

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
GIT_FOLDER="$SCRIPT_DIR/ShGame-Client"
REPO="https://github.com/Alex5X5/GatsIO-Remake.git"
BRANCH="Server"

if [ -d "$GIT_FOLDER/.git" ]; then
echo "Found existing Git repository in $GIT_FOLDER. Pulling latest changes..."
cd "$GIT_FOLDER" || exit 1
git pull -u origin "$BRANCH"
else
# If GIT_FOLDER exists but is not a Git repo, remove it
if [ -d "$GIT_FOLDER" ]; then
echo "$GIT_FOLDER exists but is not a Git repository. Replacing it..."
rm -rf "$GIT_FOLDER"
git clone -b Server "$REPO" "$GIT_FOLDER"
fi