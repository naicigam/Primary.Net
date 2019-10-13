#!/bin/sh
set -e

#export VSINSTALLDIR="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community"
#export VisualStudioVersion="15.0"

#nuget install docfx
#nuget install docfx.console

# https://github.com/dotnet/docfx/issues/3177
#nuget restore 

docfx metadata ./docs/docfx.json
docfx build ./docs/docfx.json

SOURCE_DIR=$PWD
TEMP_REPO_DIR=$PWD/../primary-net-gh-pages

echo "Removing temporary doc directory $TEMP_REPO_DIR"
rm -rf $TEMP_REPO_DIR
mkdir $TEMP_REPO_DIR

echo "Cloning the repo with the gh-pages branch"
git clone https://github.com/naicigam/Primary.Net.git --branch gh-pages $TEMP_REPO_DIR

echo "Clear repo directory"
cd $TEMP_REPO_DIR
git rm -r *

echo "Copy documentation into the repo"
cp -r $SOURCE_DIR/docs/_site/* .

echo "Push the new docs to the remote branch"
git add . -A
git commit -m "Update generated documentation"
git push origin gh-pages
