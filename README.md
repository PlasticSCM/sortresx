sortresx
========

Sorts alphabetically a .resx XML file.

Usage:

sortresx.exe resources_file.resx

This program reads the resx file content of "resources_file.resx" and re-writes it sorting the resources strings
alphabetically.

This is very useful when merging resx files that have been changed in depth: since the contents of the contributors
are sorted, manual conflicts will only arise if there are conflicts in the SAME localization strings.
