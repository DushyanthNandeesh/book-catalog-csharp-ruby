# frozen_string_literal: true

require 'json'
require 'fileutils'
require_relative 'book'
require_relative 'validation'

# Reads and writes the catalog as JSON using Ruby's standard json library.
# Ruby feature: JSON becomes plain Hashes and Arrays, so we convert each
# Hash into a Book ourselves and check the types by hand.
module Storage
  # Result of loading the data file: a status symbol plus the books.
  LoadResult = Struct.new(:status, :books)

  module_function

  def load(path)
    return LoadResult.new(:not_found, []) unless File.file?(path)

    books = parse_books(JSON.parse(File.read(path)))
    books ? LoadResult.new(:loaded, books) : LoadResult.new(:corrupt, [])
  rescue JSON::ParserError, SystemCallError, EncodingError
    LoadResult.new(:corrupt, [])
  end

  # Writes the catalog to disk. Returns false if the file cannot be written.
  def save(path, books)
    FileUtils.mkdir_p(File.dirname(File.expand_path(path)))
    File.write(path, "#{to_json_text(books)}\n")
    true
  rescue SystemCallError
    false
  end

  # Pretty-printed JSON. Some Ruby json versions print an empty array as
  # "[\n\n]", so an empty catalog is written as "[]" to match the C# version.
  def to_json_text(books)
    return '[]' if books.none?

    JSON.pretty_generate(books.map(&:to_h))
  end

  # Turns parsed JSON into Book objects, or returns nil if anything is wrong.
  def parse_books(data)
    return nil unless data.is_a?(Array)

    books = data.map do |item|
      return nil unless item.is_a?(Hash)

      Book.new(id: item['id'], title: item['title'], author: item['author'],
               genre: item['genre'], year: item['year'])
    end

    return nil unless books.all? { |book| Validation.valid_book?(book) }
    return nil unless books.map(&:id).uniq.size == books.size

    books
  end
end
